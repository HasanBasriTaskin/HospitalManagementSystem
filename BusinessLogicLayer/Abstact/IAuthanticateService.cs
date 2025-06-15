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
        Task<Response<TokenDTO>> LoginAsync(LoginDTO loginDto);
        Task<Response<TokenDTO>> RefreshTokenLoginAsync(string refreshToken);
        Task<Response<NoContentDto>> RevokeRefreshTokenAsync(string refreshToken);
    }
}
