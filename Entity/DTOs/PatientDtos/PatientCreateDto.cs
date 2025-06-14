using Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.PatientDtos
{
    public class PatientCreateDto
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(11, MinimumLength = 11, ErrorMessage = "Identity number must be 11 characters.")]
        public string IdentityNumber { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required, StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? EmergencyContact { get; set; }

        [StringLength(15)]
        public string? EmergencyPhone { get; set; }
    }
} 