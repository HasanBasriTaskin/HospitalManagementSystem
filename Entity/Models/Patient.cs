using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entity.Enums;

namespace Entity.Models
{
    public class Patient : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(11)]
        public string IdentityNumber { get; set; } = string.Empty; // TC Kimlik No
        
        [Required]
        public DateTime BirthDate { get; set; }
        
        [Required]
        public Gender Gender { get; set; }
        
        [Required]
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Email { get; set; }
        
        [MaxLength(500)]
        public string? Address { get; set; }
        
        [MaxLength(100)]
        public string? EmergencyContact { get; set; }
        
        [MaxLength(15)]
        public string? EmergencyPhone { get; set; }
        
        // Audit Fields
        public DateTime? LastVisitDate { get; set; }
        
        // Computed Properties
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        
        [NotMapped]
        public int Age => DateTime.Now.Year - BirthDate.Year - (DateTime.Now.DayOfYear < BirthDate.DayOfYear ? 1 : 0);
        
        // Navigation Properties
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual AppUser AppUser { get; set; }
    }
} 