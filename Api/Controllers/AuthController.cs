using BusinessLogicLayer.Abstact;
using Entity.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthanticateService _authService;
        
        // IPatientService dependency removed as patient registration should be handled in PatientController
        public AuthController(IAuthanticateService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result.IsSuccess)
            {
                // Set the cookie with the expiration time from the result DTO
                SetRefreshTokenToCookie(result.Data.TokenInfo.RefreshToken, result.Data.RefreshTokenExpirationTime);
                
                // Create a new response for the frontend that only contains the safe-to-display TokenDTO
                var frontendResponse = ServiceResponse<TokenDTO>.Success(result.Data.TokenInfo);
                return Ok(frontendResponse);
            }
            // Use Unauthorized for login failures
            return Unauthorized(result);
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")] // Only admins can register new users
        public async Task<IActionResult> Register(RegisterDTO registerDto, [FromQuery] string role = "Patient")
        {
            var result = await _authService.RegisterAsync(registerDto, role);
            if (result.IsSuccess)
            {
                // Return 201 Created with the response object
                return Created(nameof(Register), result);
            }
            // Use BadRequest for validation or other registration errors
            return BadRequest(result);
        }

        /* 
        // This endpoint should be in PatientController.
        // Temporarily commented out.
        [HttpPost("register-patient")]
        public async Task<IActionResult> RegisterPatient(PatientRegisterDto registerDto)
        {
            var result = await _patientService.RegisterPatientAsync(registerDto);
            if (result.IsSuccessful)
            {
                return Created(nameof(RegisterPatient), result.Data);
            }
            return BadRequest(result.Errors);
        }
        */

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(ServiceResponse<TokenDTO>.Failure("Refresh token is missing."));
            }
            
            var result = await _authService.RefreshTokenLoginAsync(refreshToken);
            if (result.IsSuccess)
            {
                // Set the cookie with the new expiration time
                SetRefreshTokenToCookie(result.Data.TokenInfo.RefreshToken, result.Data.RefreshTokenExpirationTime);

                // Create a new response for the frontend
                var frontendResponse = ServiceResponse<TokenDTO>.Success(result.Data.TokenInfo);
                return Ok(frontendResponse);
            }
            return Unauthorized(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                 await _authService.RevokeRefreshTokenAsync(refreshToken);
            }
            
            // Invalidate the cookie
            Response.Cookies.Delete("refreshToken");
            
            return Ok(new { message = "Logout successful" });
        }
        
        private void SetRefreshTokenToCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires,
                Secure = true,
                SameSite = SameSiteMode.Strict 
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
