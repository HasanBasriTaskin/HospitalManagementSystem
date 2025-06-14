using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.DoctorDtos
{
    public class DoctorCreateDto
    {
        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        [Required, StringLength(20)]
        public string LicenseNumber { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Phone { get; set; }

        [EmailAddress, StringLength(100)]
        public string? Email { get; set; }
    }
} 