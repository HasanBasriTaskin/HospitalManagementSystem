using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Entity.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<UserRefreshToken> UserRefreshTokens { get; set; }

        public int? DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public int? PatientId { get; set; }
        public Patient Patient { get; set; }
    }
} 