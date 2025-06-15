using Entity.DTOs;
using Entity.DTOs.AppointmentDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstact
{
    public interface IAppointmentService
    {
        Task<ServiceResponse<AppointmentDto>> GetByIdAsync(int id);
        Task<ServiceResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctorIdAsync(int doctorId, DateTime date);
        Task<ServiceResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByPatientIdAsync(int patientId);
        Task<ServiceResponse<IEnumerable<AvailableSlotDto>>> GetAvailableAppointmentSlotsAsync(int doctorId, DateTime date);
        Task<ServiceResponse<AppointmentDto>> CreateAsync(AppointmentCreateDto createDto);
        Task<ServiceResponse<bool>> CancelAsync(int id);
        
        // These methods are used internally by other services for business rule checks,
        // so their return type can remain as bool for simplicity.
        Task<bool> HasUpcomingAppointmentsForDoctorAsync(int doctorId);
        Task<bool> HasUpcomingAppointmentsForPatientAsync(int patientId);
    }
} 