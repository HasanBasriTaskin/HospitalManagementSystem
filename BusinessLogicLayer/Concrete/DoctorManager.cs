using AutoMapper;
using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
using Entity.DTOs.DoctorDtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Concrete
{
    public class DoctorManager : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppointmentService _appointmentService;

        public DoctorManager(IUnitOfWork unitOfWork, IMapper mapper, IAppointmentService appointmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointmentService = appointmentService;
        }

        public async Task<DoctorDto> CreateAsync(DoctorCreateDto createDto)
        {
            var departmentExists = await _unitOfWork.DepartmentRepository.ExistsAsync(d => d.Id == createDto.DepartmentId);
            if (!departmentExists)
            {
                throw new KeyNotFoundException($"Department with ID {createDto.DepartmentId} not found.");
            }

            var licenseExists = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.LicenseNumber == createDto.LicenseNumber);
            if (licenseExists)
            {
                throw new InvalidOperationException($"A doctor with license number {createDto.LicenseNumber} already exists.");
            }

            var doctor = _mapper.Map<Doctor>(createDto);

            _unitOfWork.DoctorRepository.Add(doctor);
            await _unitOfWork.SaveChangesAsync();
            
            return await GetByIdAsync(doctor.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                throw new KeyNotFoundException($"Doctor with ID {id} not found.");
            }

            // Business Rule: Cannot delete a doctor with upcoming appointments.
            var hasUpcomingAppointments = await _appointmentService.HasUpcomingAppointmentsForDoctorAsync(id);
            if (hasUpcomingAppointments)
            {
                throw new InvalidOperationException("This doctor cannot be deleted because they have upcoming appointments.");
            }

            _unitOfWork.DoctorRepository.Delete(doctor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<DoctorDto>> GetAllAsync()
        {
            var doctors = await _unitOfWork.DoctorRepository.GetAllAsync();
            // Manual mapping is required here because the generic repository doesn't support Includes.
            // This can be optimized later by enhancing the repository.
            var doctorDtos = _mapper.Map<IEnumerable<DoctorDto>>(doctors);
            
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            var departmentMap = departments.ToDictionary(d => d.Id, d => d.Name);

            foreach (var dto in doctorDtos)
            {
                if (departmentMap.TryGetValue(dto.DepartmentId, out var departmentName))
                {
                    dto.DepartmentName = departmentName;
                }
            }
            return doctorDtos;
        }

        public async Task<DoctorDto> GetByIdAsync(int id)
        {
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id);
            if (doctor == null)
            {
                throw new KeyNotFoundException($"Doctor with ID {id} not found.");
            }
            
            // Manual mapping for department name
            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(doctor.DepartmentId);
            doctorDto.DepartmentName = department?.Name ?? "N/A";
            
            return doctorDto;
        }

        public async Task UpdateAsync(DoctorUpdateDto updateDto)
        {
            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(updateDto.Id);
            if (doctor == null)
            {
                throw new KeyNotFoundException($"Doctor with ID {updateDto.Id} not found.");
            }

            var departmentExists = await _unitOfWork.DepartmentRepository.ExistsAsync(d => d.Id == updateDto.DepartmentId);
            if (!departmentExists)
            {
                throw new KeyNotFoundException($"Department with ID {updateDto.DepartmentId} not found.");
            }
            
            var licenseExists = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.LicenseNumber == updateDto.LicenseNumber && d.Id != updateDto.Id);
            if (licenseExists)
            {
                throw new InvalidOperationException($"A doctor with license number {updateDto.LicenseNumber} already exists.");
            }

            _mapper.Map(updateDto, doctor);

            _unitOfWork.DoctorRepository.Update(doctor);
            await _unitOfWork.SaveChangesAsync();
        }
    }
} 