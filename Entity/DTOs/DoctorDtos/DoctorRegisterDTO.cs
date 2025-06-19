using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.DoctorDtos
{
    public class DoctorRegisterDTO
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(20)]
        public string LicenseNumber { get; set; } = string.Empty;

        [MaxLength(15)]
        public string? Phone { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
} 