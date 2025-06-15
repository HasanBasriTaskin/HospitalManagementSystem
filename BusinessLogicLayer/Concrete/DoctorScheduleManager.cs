using AutoMapper;
using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
using Entity.DTOs;
using Entity.DTOs.DoctorScheduleDtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Concrete
{
    public class DoctorScheduleManager : IDoctorScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DoctorScheduleManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<DoctorScheduleDto>> CreateAsync(DoctorScheduleCreateDto createDto)
        {
            var validationErrors = new List<string>();

            // Rule 1: Doctor must exist
            var doctorExists = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.Id == createDto.DoctorId);
            if (!doctorExists)
            {
                validationErrors.Add($"Doctor with ID {createDto.DoctorId} not found.");
            }

            // Rule 2: EndTime must be after StartTime
            if (createDto.EndTime <= createDto.StartTime)
            {
                validationErrors.Add("End time must be after start time.");
            }

            // Rule 3: DayOfWeek must be valid (1-7)
            if(createDto.DayOfWeek < 1 || createDto.DayOfWeek > 7)
            {
                validationErrors.Add("DayOfWeek must be between 1 (Monday) and 7 (Sunday).");
            }

            // Rule 4: Check for overlapping schedules
            var existingSchedules = await _unitOfWork.DoctorScheduleRepository.FindAsync(s => s.DoctorId == createDto.DoctorId && s.DayOfWeek == createDto.DayOfWeek);
            if (existingSchedules.Any(s => createDto.StartTime < s.EndTime && createDto.EndTime > s.StartTime))
            {
                validationErrors.Add("The new schedule overlaps with an existing schedule for this doctor on the same day.");
            }

            if (validationErrors.Any())
            {
                return ServiceResponse<DoctorScheduleDto>.Failure(validationErrors);
            }

            try
            {
                var schedule = _mapper.Map<DoctorSchedule>(createDto);
                _unitOfWork.DoctorScheduleRepository.Add(schedule);
                await _unitOfWork.SaveChangesAsync();
                return await GetByIdAsync(schedule.Id);
            }
            catch(Exception ex)
            {
                return ServiceResponse<DoctorScheduleDto>.Failure($"An error occurred while creating the schedule: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var schedule = await _unitOfWork.DoctorScheduleRepository.GetByIdAsync(id);
                if (schedule == null)
                {
                    return ServiceResponse<bool>.Failure($"Schedule with ID {id} not found.");
                }

                _unitOfWork.DoctorScheduleRepository.Delete(schedule);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<bool>.Success(true);
            }
            catch(Exception ex)
            {
                return ServiceResponse<bool>.Failure($"An error occurred while deleting the schedule: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<DoctorScheduleDto>>> GetAllByDoctorIdAsync(int doctorId)
        {
            try
            {
                var schedules = await _unitOfWork.DoctorScheduleRepository.FindAsync(s => s.DoctorId == doctorId);
                var scheduleDtos = _mapper.Map<IEnumerable<DoctorScheduleDto>>(schedules);
                return ServiceResponse<IEnumerable<DoctorScheduleDto>>.Success(scheduleDtos);
            }
            catch(Exception ex)
            {
                return ServiceResponse<IEnumerable<DoctorScheduleDto>>.Failure($"An error occurred while retrieving schedules: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<DoctorScheduleDto>> GetByIdAsync(int id)
        {
            try
            {
                var schedule = await _unitOfWork.DoctorScheduleRepository.GetByIdAsync(id);
                if (schedule == null)
                {
                    return ServiceResponse<DoctorScheduleDto>.Failure($"Schedule with ID {id} not found.");
                }

                var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(schedule.DoctorId);
                var scheduleDto = _mapper.Map<DoctorScheduleDto>(schedule);
                if (doctor != null)
                {
                    scheduleDto.DoctorFullName = doctor.FullName;
                }

                return ServiceResponse<DoctorScheduleDto>.Success(scheduleDto);
            }
            catch(Exception ex)
            {
                return ServiceResponse<DoctorScheduleDto>.Failure($"An error occurred while retrieving the schedule: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(DoctorScheduleUpdateDto updateDto)
        {
            var validationErrors = new List<string>();

            var schedule = await _unitOfWork.DoctorScheduleRepository.GetByIdAsync(updateDto.Id);
            if (schedule == null)
            {
                return ServiceResponse<bool>.Failure($"Schedule with ID {updateDto.Id} not found.");
            }

            // Rule 1: EndTime must be after StartTime
            if (updateDto.EndTime <= updateDto.StartTime)
            {
                validationErrors.Add("End time must be after start time.");
            }

            // Rule 2: DayOfWeek must be valid (1-7)
            if(updateDto.DayOfWeek < 1 || updateDto.DayOfWeek > 7)
            {
                validationErrors.Add("DayOfWeek must be between 1 (Monday) and 7 (Sunday).");
            }

            // Rule 3: Check for overlapping schedules, excluding the current one
            var existingSchedules = await _unitOfWork.DoctorScheduleRepository.FindAsync(s => s.DoctorId == schedule.DoctorId && s.DayOfWeek == updateDto.DayOfWeek && s.Id != updateDto.Id);
            if (existingSchedules.Any(s => updateDto.StartTime < s.EndTime && updateDto.EndTime > s.StartTime))
            {
                validationErrors.Add("The updated schedule overlaps with another existing schedule for this doctor on the same day.");
            }

            if (validationErrors.Any())
            {
                return ServiceResponse<bool>.Failure(validationErrors);
            }
            
            try
            {
                _mapper.Map(updateDto, schedule);
                _unitOfWork.DoctorScheduleRepository.Update(schedule);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<bool>.Success(true);
            }
            catch(Exception ex)
            {
                return ServiceResponse<bool>.Failure($"An error occurred while updating the schedule: {ex.Message}");
            }
        }
    }
} 