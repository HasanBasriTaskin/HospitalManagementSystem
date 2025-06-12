using System.ComponentModel.DataAnnotations;
using Entity.Enums;

namespace Entity.Models
{
    public class Appointment : BaseEntity
    {
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [Range(15, 120)]
        public int Duration { get; set; } = 30; // in minutes
        
        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
        
        [MaxLength(1000)]
        public string? Complaint { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        // Audit Fields
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancelledBy { get; set; }
        
        [MaxLength(50)]
        public string CreatedBy { get; set; } = "System";
        
        // Navigation Properties
        public virtual Patient Patient { get; set; } = null!;
        public virtual Doctor Doctor { get; set; } = null!;
    }
} 