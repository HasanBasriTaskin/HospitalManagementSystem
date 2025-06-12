using BusinessLogicLayer.Abstact;
using Entity.DTOs;
using Entity.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth/[action]")]
    public class AuthController : Controller

    {
        private readonly IAuthanticateService authanticateService;
        private readonly UserManager<IdentityUser> userManager;

        public AuthController(IAuthanticateService authanticateService, UserManager<IdentityUser> userManager
            )
        {
            this.authanticateService = authanticateService;
            this.userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Response<TokenDTO>.Fail(400, new List<string>() { "Kullanıcı bilgisi giriniz.." }));
            }

            var token = await authanticateService.GenerateToken(loginDto);
            if (token.IsSuccessful)
            {
                var cookieOptions = authanticateService.SetCookieOptions();
                Response.Cookies.Append("JWTCookie", token.Data.Token, cookieOptions.Data);
                return Ok(Response<TokenDTO>.Success(token.Data, 200));
            }

            return BadRequest(Response<TokenDTO>.Fail(400, token.Errors));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GiveRole(string email, string roleName)
        {
            var user = await userManager.FindByEmailAsync(email);
            var result = await userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
