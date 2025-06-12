using Entity.Configrations;
using Entity.DTOs.Common;
using Entity.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Abstact;

namespace BusinessLogicLayer.Concrete.EfCore
{
    public class EfCoreAuthService : IAuthanticateService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly CustomTokenOption tokenOptions;

        public EfCoreAuthService(IOptions<CustomTokenOption> options, UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
            this.tokenOptions = options.Value;
        }

        public async Task<Response<TokenDTO>> GenerateToken(LoginDTO loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return Response<TokenDTO>.Fail(404, new List<string>() { "Kullanıcı bilgisi giriniz.." });
            }
            var user = await userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Response<TokenDTO>.Fail(404, new List<string>() { "Kullanıcı adı veya şifre yanlış!" });
            }

            var passwordCheck = await userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordCheck)
            {
                return Response<TokenDTO>.Fail(404, new List<string>() { "Kullanıcı adı veya şifre yanlış!" });
            }

            var roles = await userManager.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            };
            if (roles != null)
            {
                claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList());
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tokenOptions.SecurityKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(tokenOptions.ExpirationDay),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Response<TokenDTO>.Success(new TokenDTO()
            {
                Token = tokenHandler.WriteToken(token),
                Roles = roles.ToList(),
                User = new UserDTO()
                {
                    Id = user.Id,
                    Email = user.Email,
                },
                ExpirationTime = DateTime.UtcNow.AddDays(tokenOptions.ExpirationDay)
            }, 200);
        }
        public Response<CookieOptions> SetCookieOptions()
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(tokenOptions.ExpirationDay),
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
            };
            return Response<CookieOptions>.Success(cookieOptions, 200);
        }
    }
}
