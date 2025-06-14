using AutoMapper;
using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
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

        public async Task<DoctorScheduleDto> CreateAsync(DoctorScheduleCreateDto createDto)
        {
            // Rule 1: Doctor must exist
            var doctorExists = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.Id == createDto.DoctorId);
            if (!doctorExists)
            {
                throw new KeyNotFoundException($"Doctor with ID {createDto.DoctorId} not found.");
            }

            // Rule 2: EndTime must be after StartTime
            if (createDto.EndTime <= createDto.StartTime)
            {
                throw new InvalidOperationException("End time must be after start time.");
            }

            // Rule 3: Check for overlapping schedules for the same doctor on the same day
            var existingSchedules = await _unitOfWork.DoctorScheduleRepository.FindAsync(s => s.DoctorId == createDto.DoctorId && s.DayOfWeek == createDto.DayOfWeek);
            var hasOverlap = existingSchedules.Any(s =>
                createDto.StartTime < s.EndTime && createDto.EndTime > s.StartTime
            );

            if (hasOverlap)
            {
                throw new InvalidOperationException("The new schedule overlaps with an existing schedule for this doctor on the same day.");
            }

            var schedule = _mapper.Map<DoctorSchedule>(createDto);

            _unitOfWork.DoctorScheduleRepository.Add(schedule);
            await _unitOfWork.SaveChangesAsync();

            return await GetByIdAsync(schedule.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var schedule = await _unitOfWork.DoctorScheduleRepository.GetByIdAsync(id);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Schedule with ID {id} not found.");
            }

            _unitOfWork.DoctorScheduleRepository.Delete(schedule);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<DoctorScheduleDto>> GetAllByDoctorIdAsync(int doctorId)
        {
            var schedules = await _unitOfWork.DoctorScheduleRepository.FindAsync(s => s.DoctorId == doctorId);
            return _mapper.Map<IEnumerable<DoctorScheduleDto>>(schedules);
        }

        public async Task<DoctorScheduleDto> GetByIdAsync(int id)
        {
            var schedule = await _unitOfWork.DoctorScheduleRepository.GetByIdAsync(id);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Schedule with ID {id} not found.");
            }

            // Manually load doctor for mapping DoctorFullName
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(schedule.DoctorId);
            var scheduleDto = _mapper.Map<DoctorScheduleDto>(schedule);
            if (doctor != null)
            {
                scheduleDto.DoctorFullName = doctor.FullName;
            }

            return scheduleDto;
        }

        public async Task UpdateAsync(DoctorScheduleUpdateDto updateDto)
        {
            var schedule = await _unitOfWork.DoctorScheduleRepository.GetByIdAsync(updateDto.Id);
            if (schedule == null)
            {
                throw new KeyNotFoundException($"Schedule with ID {updateDto.Id} not found.");
            }

            // Rule 1: Doctor must exist
            var doctorExists = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.Id == updateDto.DoctorId);
            if (!doctorExists)
            {
                throw new KeyNotFoundException($"Doctor with ID {updateDto.DoctorId} not found.");
            }

            // Rule 2: EndTime must be after StartTime
            if (updateDto.EndTime <= updateDto.StartTime)
            {
                throw new InvalidOperationException("End time must be after start time.");
            }

            // Rule 3: Check for overlapping schedules, excluding the current one being updated
            var existingSchedules = await _unitOfWork.DoctorScheduleRepository.FindAsync(s => s.DoctorId == updateDto.DoctorId && s.DayOfWeek == updateDto.DayOfWeek && s.Id != updateDto.Id);
            var hasOverlap = existingSchedules.Any(s =>
                updateDto.StartTime < s.EndTime && updateDto.EndTime > s.StartTime
            );

            if (hasOverlap)
            {
                throw new InvalidOperationException("The updated schedule overlaps with another existing schedule for this doctor on the same day.");
            }

            _mapper.Map(updateDto, schedule);

            _unitOfWork.DoctorScheduleRepository.Update(schedule);
            await _unitOfWork.SaveChangesAsync();
        }
    }
} 