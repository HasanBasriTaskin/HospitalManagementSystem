using System;

namespace Entity.DTOs.Common
{
    /// <summary>
    /// This DTO is used for internal data transfer between the service layer and the controller layer.
    /// It carries information needed by the controller (like RefreshTokenExpirationTime) that should not be exposed to the frontend.
    /// </summary>
    public class LoginResultDTO
    {
        public TokenDTO TokenInfo { get; set; }
        public DateTime RefreshTokenExpirationTime { get; set; }
    }
} 