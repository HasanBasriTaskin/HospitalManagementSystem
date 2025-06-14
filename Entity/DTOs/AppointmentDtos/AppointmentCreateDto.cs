using System;
using System.ComponentModel.DataAnnotations;

namespace Entity.DTOs.AppointmentDtos
{
    public class AppointmentCreateDto
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        [StringLength(1000)]
        public string? Complaint { get; set; }
    }
} 