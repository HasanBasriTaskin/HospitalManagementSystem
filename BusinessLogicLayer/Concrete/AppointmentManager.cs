using AutoMapper;
using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
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

        public async Task<AppointmentDto> CreateAsync(AppointmentCreateDto createDto)
        {
            // Rule 1: Patient and Doctor must exist.
            var patientExists = await _unitOfWork.PatientRepository.ExistsAsync(p => p.Id == createDto.PatientId);
            if (!patientExists) throw new KeyNotFoundException($"Patient with ID {createDto.PatientId} not found.");

            var doctorExists = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.Id == createDto.DoctorId);
            if (!doctorExists) throw new KeyNotFoundException($"Doctor with ID {createDto.DoctorId} not found.");

            // Rule 2: The requested appointment slot must be available.
            var availableSlots = await GetAvailableAppointmentSlotsAsync(createDto.DoctorId, createDto.AppointmentDate);
            var isSlotAvailable = availableSlots.Any(slot => slot.StartTime == createDto.AppointmentTime);
            if (!isSlotAvailable)
            {
                throw new InvalidOperationException("The requested appointment time is not available for this doctor on this date.");
            }

            // Rule 3: Patient cannot have another appointment at the same time.
            var patientHasConflict = await _unitOfWork.AppointmentRepository.ExistsAsync(a =>
                a.PatientId == createDto.PatientId &&
                a.AppointmentDate.Date == createDto.AppointmentDate.Date &&
                a.AppointmentTime == createDto.AppointmentTime &&
                a.Status == AppointmentStatus.Scheduled);
            if (patientHasConflict)
            {
                throw new InvalidOperationException("The patient already has another appointment at this time.");
            }
            
            var schedule = await _unitOfWork.DoctorScheduleRepository.FirstOrDefaultAsync(s => s.DoctorId == createDto.DoctorId && s.DayOfWeek == (int)createDto.AppointmentDate.DayOfWeek);
            var appointment = _mapper.Map<Appointment>(createDto);
            appointment.Status = AppointmentStatus.Scheduled;
            appointment.Duration = schedule?.AppointmentDuration ?? 30; // Default to 30 mins if no schedule found

            _unitOfWork.AppointmentRepository.Add(appointment);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(appointment.Id);
        }
        
        public async Task<IEnumerable<AvailableSlotDto>> GetAvailableAppointmentSlotsAsync(int doctorId, DateTime date)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // Convert Sunday from 0 to 7

            var schedule = await _unitOfWork.DoctorScheduleRepository.FirstOrDefaultAsync(s =>
                s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek);

            if (schedule == null)
            {
                return new List<AvailableSlotDto>(); // Doctor does not work on this day
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

            return availableSlots;
        }

        public async Task<AppointmentDto> GetByIdAsync(int id)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException($"Appointment with ID {id} not found.");
            }
            
            // Manually load related entities for mapping
            appointment.Patient = await _unitOfWork.PatientRepository.GetByIdAsync(appointment.PatientId);
            appointment.Doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(appointment.DoctorId);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorIdAsync(int doctorId, DateTime date)
        {
            var appointments = await _unitOfWork.AppointmentRepository.FindAsync(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date);
            foreach(var app in appointments)
            {
                app.Patient = await _unitOfWork.PatientRepository.GetByIdAsync(app.PatientId);
            }
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            var appointments = await _unitOfWork.AppointmentRepository.FindAsync(a => a.PatientId == patientId);
            foreach(var app in appointments)
            {
                app.Doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(app.DoctorId);
            }
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task CancelAsync(int id)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null)
            {
                throw new KeyNotFoundException($"Appointment with ID {id} not found.");
            }
            
            // Business Rule: Cannot cancel an appointment that is less than 24 hours away.
            var appointmentDateTime = appointment.AppointmentDate.Add(appointment.AppointmentTime);
            if ((appointmentDateTime - DateTime.Now).TotalHours < 24)
            {
                throw new InvalidOperationException("Appointments cannot be cancelled less than 24 hours in advance.");
            }

            if (appointment.Status == AppointmentStatus.Cancelled)
            {
                 throw new InvalidOperationException("This appointment has already been cancelled.");
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.CancelledDate = DateTime.Now;
            // appointment.CancelledBy = ... // This would come from user context

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.SaveChangesAsync();
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