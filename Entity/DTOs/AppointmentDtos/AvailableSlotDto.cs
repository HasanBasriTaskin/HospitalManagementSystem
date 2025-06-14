using System;

namespace Entity.DTOs.AppointmentDtos
{
    public class AvailableSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
} 