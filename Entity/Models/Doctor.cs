using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity.Models
{
    public class Doctor : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string Title { get; set; } = string.Empty; // Dr., Prof. Dr., Doç. Dr.
        
        [Required]
        public int DepartmentId { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string LicenseNumber { get; set; } = string.Empty;
        
        [MaxLength(15)]
        public string? Phone { get; set; }
        
        [MaxLength(100)]
        public string? Email { get; set; }
        
        // Computed Properties
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        
        [NotMapped]
        public string DisplayName => $"{Title} {FullName}";
        
        // Navigation Properties
        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual AppUser AppUser { get; set; }
    }
} 