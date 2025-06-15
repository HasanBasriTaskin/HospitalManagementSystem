using Entity.Models;

namespace Entity.Models
{
    public class UserRefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public AppUser User { get; set; }
    }
} 