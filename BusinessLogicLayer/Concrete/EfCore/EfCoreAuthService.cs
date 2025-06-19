using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
using Entity.Configrations;
using Entity.DTOs.Common;
using Entity.DTOs.DoctorDtos;
using Entity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Concrete.EfCore
{
    public class EfCoreAuthService : IAuthanticateService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CustomTokenOption _tokenOptions;

        public EfCoreAuthService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IOptions<CustomTokenOption> tokenOptions)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _tokenOptions = tokenOptions.Value;
        }

        public async Task<ServiceResponse<LoginResultDTO>> LoginAsync(LoginDTO loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return ServiceResponse<LoginResultDTO>.Failure("Email and password are required.");
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return ServiceResponse<LoginResultDTO>.Failure("Invalid email or password.");
            }

            var accessToken = await CreateAccessToken(user);
            var newRefreshToken = CreateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenExpiration);
            
            var userRefreshToken = await _unitOfWork.UserRefreshTokenRepository.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (userRefreshToken != null)
            {
                userRefreshToken.Token = newRefreshToken;
                userRefreshToken.Expiration = refreshTokenExpiration;
                _unitOfWork.UserRefreshTokenRepository.Update(userRefreshToken);
            }
            else
            {
                _unitOfWork.UserRefreshTokenRepository.Add(new UserRefreshToken
                {
                    UserId = user.Id,
                    Token = newRefreshToken,
                    Expiration = refreshTokenExpiration
                });
            }

            await _unitOfWork.SaveChangesAsync();
            
            accessToken.RefreshToken = newRefreshToken;
            
            var loginResult = new LoginResultDTO
            {
                TokenInfo = accessToken,
                RefreshTokenExpirationTime = refreshTokenExpiration
            };

            return ServiceResponse<LoginResultDTO>.Success(loginResult);
        }

        public async Task<ServiceResponse<LoginResultDTO>> RefreshTokenLoginAsync(string refreshToken)
        {
            var userRefreshToken = await _unitOfWork.UserRefreshTokenRepository
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (userRefreshToken == null || userRefreshToken.Expiration <= DateTime.UtcNow)
            {
                return ServiceResponse<LoginResultDTO>.Failure("Refresh token not found or expired.");
            }

            var user = await _userManager.FindByIdAsync(userRefreshToken.UserId);
            if (user == null)
            {
                return ServiceResponse<LoginResultDTO>.Failure("User not found.");
            }

            // Token Rotation
            var newAccessToken = await CreateAccessToken(user);
            var newRefreshToken = CreateRefreshToken();
            var newRefreshTokenExpiration = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenExpiration);
            
            userRefreshToken.Token = newRefreshToken;
            userRefreshToken.Expiration = newRefreshTokenExpiration;

            _unitOfWork.UserRefreshTokenRepository.Update(userRefreshToken);
            await _unitOfWork.SaveChangesAsync();
            
            newAccessToken.RefreshToken = newRefreshToken;

            var loginResult = new LoginResultDTO
            {
                TokenInfo = newAccessToken,
                RefreshTokenExpirationTime = newRefreshTokenExpiration
            };

            return ServiceResponse<LoginResultDTO>.Success(loginResult);
        }

        public async Task<ServiceResponse<bool>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var userRefreshToken = await _unitOfWork.UserRefreshTokenRepository
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (userRefreshToken == null)
            {
                return ServiceResponse<bool>.Failure("Refresh token not found.");
            }

            _unitOfWork.UserRefreshTokenRepository.Delete(userRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<bool>.Success(true);
        }

        private async Task<TokenDTO> CreateAccessToken(AppUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_tokenOptions.SecurityKey));
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenOptions.AccessTokenExpiration),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return new TokenDTO
            {
                Token = tokenHandler.WriteToken(token),
                ExpirationTime = tokenDescriptor.Expires.Value,
                Roles = userRoles.ToList(),
                 User = new UserDTO()
                {
                    Id = user.Id,
                    Email = user.Email,
                }
            };
        }

        private string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<ServiceResponse<UserDTO>> RegisterDoctorAsync(DoctorRegisterDTO dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Create Doctor entity
                var doctor = new Doctor
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Title = dto.Title,
                    DepartmentId = dto.DepartmentId,
                    LicenseNumber = dto.LicenseNumber,
                    Phone = dto.Phone,
                    Email = dto.Email,
                };

                // Create AppUser entity and link it to the Doctor
                var appUser = new AppUser
                {
                    Email = dto.Email,
                    UserName = dto.Email,
                    Doctor = doctor 
                };

                // Create the user in Identity
                var identityResult = await _userManager.CreateAsync(appUser, dto.Password);
                if (!identityResult.Succeeded)
                {
                    var errors = identityResult.Errors.Select(e => e.Description).ToList();
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResponse<UserDTO>.Failure(errors);
                }

                // Assign the "Doctor" role
                var roleResult = await _userManager.AddToRoleAsync(appUser, "Doctor");
                if (!roleResult.Succeeded)
                {
                    var errors = roleResult.Errors.Select(e => e.Description).ToList();
                    await _unitOfWork.RollbackTransactionAsync();
                    return ServiceResponse<UserDTO>.Failure(errors);
                }

                await _unitOfWork.CommitTransactionAsync();

                var userDto = new UserDTO { Id = appUser.Id, Email = appUser.Email };
                return ServiceResponse<UserDTO>.Success(userDto);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                // Optionally log the exception ex
                return ServiceResponse<UserDTO>.Failure("An unexpected error occurred during registration.");
            }
        }
    }
}
