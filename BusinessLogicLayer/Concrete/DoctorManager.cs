using AutoMapper;
using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
using Entity.DTOs;
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

        public async Task<ServiceResponse<DoctorDto>> CreateAsync(DoctorCreateDto createDto)
        {
            var validationErrors = new List<string>();

            // Rule 1: Department must exist.
            var departmentExists = await _unitOfWork.DepartmentRepository.ExistsAsync(d => d.Id == createDto.DepartmentId);
            if (!departmentExists)
            {
                validationErrors.Add($"Department with ID {createDto.DepartmentId} not found.");
            }

            // Rule 2: License number must be unique.
            var licenseExists = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.LicenseNumber == createDto.LicenseNumber);
            if (licenseExists)
            {
                validationErrors.Add($"A doctor with license number {createDto.LicenseNumber} already exists.");
            }

            // Return if there are any validation errors
            if (validationErrors.Any())
            {
                return ServiceResponse<DoctorDto>.Failure(validationErrors);
            }
            
            try
            {
                var doctor = _mapper.Map<Doctor>(createDto);
                _unitOfWork.DoctorRepository.Add(doctor);
                await _unitOfWork.SaveChangesAsync();
                
                // Fetch the created doctor with department details to return
                return await GetByIdAsync(doctor.Id);
            }
            catch (Exception ex)
            {
                return ServiceResponse<DoctorDto>.Failure($"An error occurred while creating the doctor: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id);
                if (doctor == null)
                {
                    return ServiceResponse<bool>.Failure($"Doctor with ID {id} not found.");
                }

                // Business Rule: Cannot delete a doctor with upcoming appointments.
                var hasUpcomingAppointments = await _appointmentService.HasUpcomingAppointmentsForDoctorAsync(id);
                if (hasUpcomingAppointments)
                {
                    return ServiceResponse<bool>.Failure("This doctor cannot be deleted because they have upcoming appointments.");
                }

                _unitOfWork.DoctorRepository.Delete(doctor);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<bool>.Success(true);
            }
            catch(Exception ex)
            {
                return ServiceResponse<bool>.Failure($"An error occurred while deleting the doctor: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<DoctorDto>>> GetAllAsync()
        {
            try
            {
                var doctors = await _unitOfWork.DoctorRepository.GetAllAsync();
                var doctorDtos = _mapper.Map<IEnumerable<DoctorDto>>(doctors);
                
                // This part can be optimized. For now, we'll keep it to resolve department names.
                var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
                var departmentMap = departments.ToDictionary(d => d.Id, d => d.Name);

                foreach (var dto in doctorDtos)
                {
                    if (departmentMap.TryGetValue(dto.DepartmentId, out var departmentName))
                    {
                        dto.DepartmentName = departmentName;
                    }
                }
                return ServiceResponse<IEnumerable<DoctorDto>>.Success(doctorDtos);
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<DoctorDto>>.Failure($"An error occurred while retrieving doctors: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<DoctorDto>> GetByIdAsync(int id)
        {
            try
            {
                var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id);
                if (doctor == null)
                {
                    return ServiceResponse<DoctorDto>.Failure($"Doctor with ID {id} not found.");
                }
                
                var doctorDto = _mapper.Map<DoctorDto>(doctor);
                var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(doctor.DepartmentId);
                doctorDto.DepartmentName = department?.Name ?? "N/A";
                
                return ServiceResponse<DoctorDto>.Success(doctorDto);
            }
            catch(Exception ex)
            {
                return ServiceResponse<DoctorDto>.Failure($"An error occurred while retrieving the doctor: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(DoctorUpdateDto updateDto)
        {
            var validationErrors = new List<string>();

            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(updateDto.Id);
            if (doctor == null)
            {
                return ServiceResponse<bool>.Failure($"Doctor with ID {updateDto.Id} not found.");
            }

            var departmentExists = await _unitOfWork.DepartmentRepository.ExistsAsync(d => d.Id == updateDto.DepartmentId);
            if (!departmentExists)
            {
                validationErrors.Add($"Department with ID {updateDto.DepartmentId} not found.");
            }
            
            var licenseExists = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.LicenseNumber == updateDto.LicenseNumber && d.Id != updateDto.Id);
            if (licenseExists)
            {
                validationErrors.Add($"A doctor with license number {updateDto.LicenseNumber} already exists.");
            }
            
            if (validationErrors.Any())
            {
                return ServiceResponse<bool>.Failure(validationErrors);
            }

            try
            {
                _mapper.Map(updateDto, doctor);
                _unitOfWork.DoctorRepository.Update(doctor);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<bool>.Success(true);
            }
            catch(Exception ex)
            {
                return ServiceResponse<bool>.Failure($"An error occurred while updating the doctor: {ex.Message}");
            }
        }
    }
} 