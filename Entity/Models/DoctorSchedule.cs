using System.ComponentModel.DataAnnotations;

namespace Entity.Models
{
    public class DoctorSchedule : BaseEntity
    {
        [Required]
        public int DoctorId { get; set; }
        
        [Required]
        [Range(1, 7)] // 1=Monday, 7=Sunday
        public int DayOfWeek { get; set; }
        
        [Required]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        public TimeSpan EndTime { get; set; }
        
        [Required]
        [Range(15, 60)]
        public int AppointmentDuration { get; set; } = 30; // in minutes
        
        // Navigation Properties
        public virtual Doctor Doctor { get; set; } = null!;
    }
} 