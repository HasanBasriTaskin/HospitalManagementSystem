using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs.Common
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email adresinizi giriniz..")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Lütfen email adresinizi doğru giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifrenizi giriniz")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifrenizi tekrar giriniz..")]
        [Compare("Password", ErrorMessage = "Şifrenizi kontrol ediniz..")]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }
    }
}
