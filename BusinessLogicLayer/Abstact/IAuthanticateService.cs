using Entity.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Entity.DTOs.Common;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Abstact
{
    public interface IAuthanticateService
    {
        public Task<Response<TokenDTO>> GenerateToken(LoginDTO loginDto);
        public Response<CookieOptions> SetCookieOptions();
    }
}
