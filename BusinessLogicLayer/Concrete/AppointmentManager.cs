using AutoMapper;
using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
using Entity.DTOs;
using Entity.DTOs.AppointmentDtos;
using Entity.Enums;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Concrete
{
    public class AppointmentManager : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<AppointmentDto>> CreateAsync(AppointmentCreateDto createDto)
        {
            var validationErrors = new List<string>();

            // Rule 1: Patient and Doctor must exist.
            if (!await _unitOfWork.PatientRepository.ExistsAsync(p => p.Id == createDto.PatientId))
                validationErrors.Add($"Patient with ID {createDto.PatientId} not found.");

            if (!await _unitOfWork.DoctorRepository.ExistsAsync(d => d.Id == createDto.DoctorId))
                validationErrors.Add($"Doctor with ID {createDto.DoctorId} not found.");
            
            // If patient or doctor not found, no need to check other rules.
            if (validationErrors.Any()) return ServiceResponse<AppointmentDto>.Failure(validationErrors);

            // Rule 2: The requested appointment slot must be available.
            var availableSlotsResponse = await GetAvailableAppointmentSlotsAsync(createDto.DoctorId, createDto.AppointmentDate);
            if (!availableSlotsResponse.IsSuccess || !availableSlotsResponse.Data.Any(slot => slot.StartTime == createDto.AppointmentTime))
            {
                validationErrors.Add("The requested appointment time is not available for this doctor on this date.");
            }

            // Rule 3: Patient cannot have another appointment at the same time.
            if (await _unitOfWork.AppointmentRepository.ExistsAsync(a =>
                a.PatientId == createDto.PatientId &&
                a.AppointmentDate.Date == createDto.AppointmentDate.Date &&
                a.AppointmentTime == createDto.AppointmentTime &&
                a.Status == AppointmentStatus.Scheduled))
            {
                validationErrors.Add("The patient already has another appointment at this time.");
            }
            
            if (validationErrors.Any()) return ServiceResponse<AppointmentDto>.Failure(validationErrors);
            
            try
            {
                var schedule = await _unitOfWork.DoctorScheduleRepository.FirstOrDefaultAsync(s => s.DoctorId == createDto.DoctorId && s.DayOfWeek == (int)createDto.AppointmentDate.DayOfWeek);
                var appointment = _mapper.Map<Appointment>(createDto);
                appointment.Status = AppointmentStatus.Scheduled;
                appointment.Duration = schedule?.AppointmentDuration ?? 30; // Default to 30 mins if no schedule found

                _unitOfWork.AppointmentRepository.Add(appointment);
                await _unitOfWork.SaveChangesAsync();

                return await GetByIdAsync(appointment.Id);
            }
            catch (Exception ex)
            {
                return ServiceResponse<AppointmentDto>.Failure($"An unexpected error occurred while creating the appointment: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<AvailableSlotDto>>> GetAvailableAppointmentSlotsAsync(int doctorId, DateTime date)
        {
            try
            {
                var dayOfWeek = (int)date.DayOfWeek;
                if (dayOfWeek == 0) dayOfWeek = 7; // Convert Sunday from 0 to 7

                var schedule = await _unitOfWork.DoctorScheduleRepository.FirstOrDefaultAsync(s =>
                    s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek);

                if (schedule == null)
                {
                    return ServiceResponse<IEnumerable<AvailableSlotDto>>.Success(new List<AvailableSlotDto>()); // Doctor does not work on this day
                }

                var existingAppointments = await _unitOfWork.AppointmentRepository.FindAsync(a =>
                    a.DoctorId == doctorId &&
                    a.AppointmentDate.Date == date.Date &&
                    a.Status == AppointmentStatus.Scheduled);

                var bookedSlots = new HashSet<TimeSpan>(existingAppointments.Select(a => a.AppointmentTime));

                var availableSlots = new List<AvailableSlotDto>();
                var currentTime = schedule.StartTime;
                while (currentTime < schedule.EndTime)
                {
                    if (!bookedSlots.Contains(currentTime))
                    {
                        availableSlots.Add(new AvailableSlotDto
                        {
                            StartTime = currentTime,
                            EndTime = currentTime.Add(TimeSpan.FromMinutes(schedule.AppointmentDuration))
                        });
                    }
                    currentTime = currentTime.Add(TimeSpan.FromMinutes(schedule.AppointmentDuration));
                }
                return ServiceResponse<IEnumerable<AvailableSlotDto>>.Success(availableSlots);
            }
            catch(Exception ex)
            {
                return ServiceResponse<IEnumerable<AvailableSlotDto>>.Failure($"An error occurred while getting available slots: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<AppointmentDto>> GetByIdAsync(int id)
        {
            try
            {
                var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return ServiceResponse<AppointmentDto>.Failure($"Appointment with ID {id} not found.");
                }

                appointment.Patient = await _unitOfWork.PatientRepository.GetByIdAsync(appointment.PatientId);
                appointment.Doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(appointment.DoctorId);

                var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
                return ServiceResponse<AppointmentDto>.Success(appointmentDto);
            }
            catch(Exception ex)
            {
                return ServiceResponse<AppointmentDto>.Failure($"An error occurred while retrieving the appointment: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByDoctorIdAsync(int doctorId, DateTime date)
        {
            try
            {
                var appointments = await _unitOfWork.AppointmentRepository.FindAsync(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date);
                foreach (var app in appointments)
                {
                    app.Patient = await _unitOfWork.PatientRepository.GetByIdAsync(app.PatientId);
                }
                var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return ServiceResponse<IEnumerable<AppointmentDto>>.Success(appointmentDtos);
            }
            catch(Exception ex)
            {
                return ServiceResponse<IEnumerable<AppointmentDto>>.Failure($"An error occurred while retrieving appointments: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            try
            {
                var appointments = await _unitOfWork.AppointmentRepository.FindAsync(a => a.PatientId == patientId);
                foreach (var app in appointments)
                {
                    app.Doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(app.DoctorId);
                }
                var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
                return ServiceResponse<IEnumerable<AppointmentDto>>.Success(appointmentDtos);
            }
            catch(Exception ex)
            {
                return ServiceResponse<IEnumerable<AppointmentDto>>.Failure($"An error occurred while retrieving appointments: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> CancelAsync(int id)
        {
            var validationErrors = new List<string>();
            try
            {
                var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return ServiceResponse<bool>.Failure($"Appointment with ID {id} not found.");
                }

                var appointmentDateTime = appointment.AppointmentDate.Add(appointment.AppointmentTime);
                if ((appointmentDateTime - DateTime.Now).TotalHours < 24)
                {
                    validationErrors.Add("Appointments cannot be cancelled less than 24 hours in advance.");
                }

                if (appointment.Status == AppointmentStatus.Cancelled)
                {
                    validationErrors.Add("This appointment has already been cancelled.");
                }
                
                if (validationErrors.Any()) return ServiceResponse<bool>.Failure(validationErrors);

                appointment.Status = AppointmentStatus.Cancelled;
                appointment.CancelledDate = DateTime.Now;

                _unitOfWork.AppointmentRepository.Update(appointment);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<bool>.Success(true);
            }
            catch(Exception ex)
            {
                return ServiceResponse<bool>.Failure($"An error occurred while cancelling the appointment: {ex.Message}");
            }
        }

        public async Task<bool> HasUpcomingAppointmentsForDoctorAsync(int doctorId)
        {
            return await _unitOfWork.AppointmentRepository.ExistsAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date >= DateTime.Today &&
                a.Status == AppointmentStatus.Scheduled);
        }

        public async Task<bool> HasUpcomingAppointmentsForPatientAsync(int patientId)
        {
            return await _unitOfWork.AppointmentRepository.ExistsAsync(a =>
                a.PatientId == patientId &&
                a.AppointmentDate.Date >= DateTime.Today &&
                a.Status == AppointmentStatus.Scheduled);
        }
    }
} 