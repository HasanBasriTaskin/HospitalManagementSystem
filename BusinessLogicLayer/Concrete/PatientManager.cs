using AutoMapper;
using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
using Entity.DTOs;
using Entity.DTOs.PatientDtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Concrete
{
    public class PatientManager : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppointmentService _appointmentService;

        public PatientManager(IUnitOfWork unitOfWork, IMapper mapper, IAppointmentService appointmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointmentService = appointmentService;
        }

        public async Task<ServiceResponse<PatientDto>> CreateAsync(PatientCreateDto createDto)
        {
            // List to collect all validation errors
            var validationErrors = new List<string>();

            // Identity number check
            var identityExists = await _unitOfWork.PatientRepository.ExistsAsync(p => p.IdentityNumber == createDto.IdentityNumber);
            if (identityExists)
            {
                validationErrors.Add($"A patient with identity number {createDto.IdentityNumber} already exists.");
            }

            // Additional validation checks can be added here
            if (string.IsNullOrWhiteSpace(createDto.FirstName))
            {
                validationErrors.Add("First name is required.");
            }
            
            if (string.IsNullOrWhiteSpace(createDto.LastName))
            {
                validationErrors.Add("Last name is required.");
            }
            
            if (string.IsNullOrWhiteSpace(createDto.IdentityNumber) || createDto.IdentityNumber.Length != 11)
            {
                validationErrors.Add("Identity number must be 11 characters long.");
            }
            
            if (createDto.BirthDate > DateTime.Now)
            {
                validationErrors.Add("Birth date cannot be in the future.");
            }
            
            // Return if there are validation errors
            if (validationErrors.Count > 0)
            {
                return ServiceResponse<PatientDto>.Failure(validationErrors);
            }

            // Validation passed, proceed with operation
            try
            {
                var patient = _mapper.Map<Patient>(createDto);
                _unitOfWork.PatientRepository.Add(patient);
                await _unitOfWork.SaveChangesAsync();
                
                var patientDto = _mapper.Map<PatientDto>(patient);
                return ServiceResponse<PatientDto>.Success(patientDto);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PatientDto>.Failure($"An error occurred while creating the patient: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id);
                if (patient == null)
                {
                    return ServiceResponse<bool>.Failure($"Patient with ID {id} not found.");
                }

                // Business Rule: Check for future appointments before deleting.
                var hasUpcomingAppointments = await _appointmentService.HasUpcomingAppointmentsForPatientAsync(id);
                if (hasUpcomingAppointments)
                {
                    return ServiceResponse<bool>.Failure("This patient cannot be deleted, there are upcoming appointments.");
                }

                _unitOfWork.PatientRepository.Delete(patient);
                await _unitOfWork.SaveChangesAsync();
                
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Failure($"An error occurred while deleting the patient: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<PatientDto>>> GetAllAsync()
        {
            try
            {
                var patients = await _unitOfWork.PatientRepository.GetAllAsync();
                var patientDtos = _mapper.Map<IEnumerable<PatientDto>>(patients);
                return ServiceResponse<IEnumerable<PatientDto>>.Success(patientDtos);
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<PatientDto>>.Failure($"An error occurred while retrieving patients: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<PatientDto>> GetByIdAsync(int id)
        {
            try
            {
                var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id);
                if (patient == null)
                {
                    return ServiceResponse<PatientDto>.Failure($"Patient with ID {id} not found.");
                }

                var patientDto = _mapper.Map<PatientDto>(patient);
                return ServiceResponse<PatientDto>.Success(patientDto);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PatientDto>.Failure($"An error occurred while retrieving the patient: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(PatientUpdateDto updateDto)
        {
            // List to collect all validation errors
            var validationErrors = new List<string>();
            
            try
            {
                var patient = await _unitOfWork.PatientRepository.GetByIdAsync(updateDto.Id);
                if (patient == null)
                {
                    return ServiceResponse<bool>.Failure($"Patient with ID {updateDto.Id} not found.");
                }

                // Identity number check
                var identityExists = await _unitOfWork.PatientRepository.ExistsAsync(p => p.IdentityNumber == updateDto.IdentityNumber && p.Id != updateDto.Id);
                if (identityExists)
                {
                    validationErrors.Add($"A patient with identity number {updateDto.IdentityNumber} already exists.");
                }
                
                // Additional validation checks can be added here
                if (string.IsNullOrWhiteSpace(updateDto.FirstName))
                {
                    validationErrors.Add("First name is required.");
                }
                
                if (string.IsNullOrWhiteSpace(updateDto.LastName))
                {
                    validationErrors.Add("Last name is required.");
                }
                
                if (string.IsNullOrWhiteSpace(updateDto.IdentityNumber) || updateDto.IdentityNumber.Length != 11)
                {
                    validationErrors.Add("Identity number must be 11 characters long.");
                }
                
                if (updateDto.BirthDate > DateTime.Now)
                {
                    validationErrors.Add("Birth date cannot be in the future.");
                }
                
                // Return if there are validation errors
                if (validationErrors.Count > 0)
                {
                    return ServiceResponse<bool>.Failure(validationErrors);
                }

                _mapper.Map(updateDto, patient);
                _unitOfWork.PatientRepository.Update(patient);
                await _unitOfWork.SaveChangesAsync();
                
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Failure($"An error occurred while updating the patient: {ex.Message}");
            }
        }
    }
} 