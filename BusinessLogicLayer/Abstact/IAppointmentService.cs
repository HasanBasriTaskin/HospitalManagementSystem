using Entity.DTOs.AppointmentDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstact
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> GetByIdAsync(int id);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorIdAsync(int doctorId, DateTime date);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientIdAsync(int patientId);
        Task<IEnumerable<AvailableSlotDto>> GetAvailableAppointmentSlotsAsync(int doctorId, DateTime date);
        Task<AppointmentDto> CreateAsync(AppointmentCreateDto createDto);
        Task CancelAsync(int id);
        // Maybe an UpdateAsync for notes later
    }
} 