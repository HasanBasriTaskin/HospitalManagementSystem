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
        Task<ServiceResponse<LoginResultDTO>> LoginAsync(LoginDTO loginDto);
        Task<ServiceResponse<UserDTO>> RegisterAsync(RegisterDTO registerDto, string role);
        Task<ServiceResponse<LoginResultDTO>> RefreshTokenLoginAsync(string refreshToken);
        Task<ServiceResponse<bool>> RevokeRefreshTokenAsync(string refreshToken);
    }
}
