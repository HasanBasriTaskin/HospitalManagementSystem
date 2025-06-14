using Entity.Enums;
using System;

namespace Entity.DTOs.AppointmentDtos
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public string DoctorFullName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Complaint { get; set; }
        public string? Notes { get; set; }
    }
} 