using AutoMapper;
using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
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

        public PatientManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PatientDto> CreateAsync(PatientCreateDto createDto)
        {
            var identityExists = await _unitOfWork.PatientRepository.ExistsAsync(p => p.IdentityNumber == createDto.IdentityNumber);
            if (identityExists)
            {
                throw new InvalidOperationException($"A patient with identity number {createDto.IdentityNumber} already exists.");
            }

            var patient = _mapper.Map<Patient>(createDto);

            _unitOfWork.PatientRepository.Add(patient);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task DeleteAsync(int id)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }

            // Business Rule: Check for future appointments before deleting.
            // Will be implemented with AppointmentService.
            // var hasUpcomingAppointments = await _unitOfWork.AppointmentRepository.ExistsAsync(a => a.PatientId == id && a.AppointmentDate >= DateTime.Today);
            // if (hasUpcomingAppointments)
            // {
            //     throw new InvalidOperationException("This patient cannot be deleted, there are upcoming appointments.");
            // }

            _unitOfWork.PatientRepository.Delete(patient);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PatientDto>> GetAllAsync()
        {
            var patients = await _unitOfWork.PatientRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }

        public async Task<PatientDto> GetByIdAsync(int id)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {id} not found.");
            }

            return _mapper.Map<PatientDto>(patient);
        }

        public async Task UpdateAsync(PatientUpdateDto updateDto)
        {
            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(updateDto.Id);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Patient with ID {updateDto.Id} not found.");
            }

            var identityExists = await _unitOfWork.PatientRepository.ExistsAsync(p => p.IdentityNumber == updateDto.IdentityNumber && p.Id != updateDto.Id);
            if (identityExists)
            {
                throw new InvalidOperationException($"A patient with identity number {updateDto.IdentityNumber} already exists.");
            }

            _mapper.Map(updateDto, patient);

            _unitOfWork.PatientRepository.Update(patient);
            await _unitOfWork.SaveChangesAsync();
        }
    }
} 