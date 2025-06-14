using System;
using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.DoctorScheduleDtos
{
    public class DoctorScheduleCreateDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        [Range(1, 7, ErrorMessage = "DayOfWeek must be between 1 (Monday) and 7 (Sunday).")]
        public int DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Range(15, 60, ErrorMessage = "Appointment duration must be between 15 and 60 minutes.")]
        public int AppointmentDuration { get; set; }
    }
} 