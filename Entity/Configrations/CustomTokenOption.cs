using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Configrations
{
    public class CustomTokenOption
    {
        public int AccessTokenExpiration { get; set; } // minutes
        public int RefreshTokenExpiration { get; set; } // days
        public string SecurityKey { get; set; }
    }
}
