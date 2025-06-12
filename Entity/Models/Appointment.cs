using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entity.Enums;

namespace Entity.Models
{
    public class Appointment : BaseEntity
    {
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public int DoctorId { get; set; }
        
        /// The date when the appointment is scheduled
        /// Example: 2024-03-20
        [Required]
        public DateTime AppointmentDate { get; set; }
        
        /// The start time of the appointment
        /// Example: 14:30 (2:30 PM)
        [Required]
        public TimeSpan AppointmentTime { get; set; }
        
        /// Duration of the appointment in minutes
        /// Default: 30 minutes
        /// Range: 15-120 minutes
        /// Used to calculate the end time of the appointment
        /// Example: If AppointmentTime is 14:30 and Duration is 45,
        /// then the appointment ends at 15:15
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
        [ForeignKey("DoctorId")]
        public virtual Doctor? Doctor { get; set; }
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }
} 