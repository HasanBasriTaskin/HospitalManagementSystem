using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Common
{
    public class TokenDTO
    {
        public string Token { get; set; }
        public UserDTO User { get; set; }
        public List<string> Roles { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
