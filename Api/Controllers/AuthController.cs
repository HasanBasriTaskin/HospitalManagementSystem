using BusinessLogicLayer.Abstact;
using Entity.DTOs.Common;
using Entity.DTOs.PatientDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthanticateService _authService;
        private readonly IPatientService _patientService;

        public AuthController(IAuthanticateService authService, IPatientService patientService)
        {
            _authService = authService;
            _patientService = patientService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result.IsSuccessful)
            {
                SetRefreshTokenToCookie(result.Data.RefreshToken);
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")] // Only admins can register new users
        public async Task<IActionResult> Register(RegisterDTO registerDto, [FromQuery] string role = "Patient")
        {
            var result = await _authService.RegisterAsync(registerDto, role);
            if (result.IsSuccessful)
            {
                return Created(nameof(Register), result.Data);
            }
            return BadRequest(result.Errors);
        }

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

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _authService.RefreshTokenLoginAsync(refreshToken);
            if (result.IsSuccessful)
            {
                SetRefreshTokenToCookie(result.Data.RefreshToken);
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _authService.RevokeRefreshTokenAsync(refreshToken);
            
            // Invalidate the cookie
            Response.Cookies.Delete("refreshToken");

            if (result.IsSuccessful)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }
        
        private void SetRefreshTokenToCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = System.DateTime.UtcNow.AddDays(7), // Should match refresh token's lifetime
                Secure = true, // Should be true in production
                SameSite = SameSiteMode.Strict 
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
