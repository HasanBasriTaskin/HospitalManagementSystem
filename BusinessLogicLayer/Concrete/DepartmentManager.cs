using AutoMapper;
using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
using Entity.DTOs;
using Entity.DTOs.DepartmentDtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Concrete
{
    public class DepartmentManager : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<DepartmentDto>> CreateAsync(DepartmentCreateDto createDto)
        {
            try
            {
                var isExists = await _unitOfWork.DepartmentRepository.ExistsAsync(d => d.Name.ToLower() == createDto.Name.ToLower());
                if (isExists)
                {
                    return ServiceResponse<DepartmentDto>.Failure("A department with this name already exists.");
                }

                var department = _mapper.Map<Department>(createDto);
                _unitOfWork.DepartmentRepository.Add(department);
                await _unitOfWork.SaveChangesAsync();

                var departmentDto = _mapper.Map<DepartmentDto>(department);
                return ServiceResponse<DepartmentDto>.Success(departmentDto);
            }
            catch (Exception ex)
            {
                return ServiceResponse<DepartmentDto>.Failure($"An error occurred while creating the department: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id);
                if (department == null)
                {
                    return ServiceResponse<bool>.Failure($"Department with ID {id} not found.");
                }

                // Business Rule: A department cannot be deleted if it has doctors.
                var hasDoctors = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.DepartmentId == id);
                if (hasDoctors)
                {
                    return ServiceResponse<bool>.Failure("This department cannot be deleted because it has doctors assigned to it.");
                }

                _unitOfWork.DepartmentRepository.Delete(department);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Failure($"An error occurred while deleting the department: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<IEnumerable<DepartmentDto>>> GetAllAsync()
        {
            try
            {
                var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
                var departmentDtos = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
                return ServiceResponse<IEnumerable<DepartmentDto>>.Success(departmentDtos);
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<DepartmentDto>>.Failure($"An error occurred while retrieving departments: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<DepartmentDto>> GetByIdAsync(int id)
        {
            try
            {
                var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id);
                if (department == null)
                {
                    return ServiceResponse<DepartmentDto>.Failure($"Department with ID {id} not found.");
                }

                var departmentDto = _mapper.Map<DepartmentDto>(department);
                return ServiceResponse<DepartmentDto>.Success(departmentDto);
            }
            catch (Exception ex)
            {
                return ServiceResponse<DepartmentDto>.Failure($"An error occurred while retrieving the department: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(DepartmentUpdateDto updateDto)
        {
            try
            {
                var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(updateDto.Id);
                if (department == null)
                {
                    return ServiceResponse<bool>.Failure($"Department with ID {updateDto.Id} not found.");
                }

                var isExists = await _unitOfWork.DepartmentRepository.ExistsAsync(d => d.Name.ToLower() == updateDto.Name.ToLower() && d.Id != updateDto.Id);
                if (isExists)
                {
                    return ServiceResponse<bool>.Failure("A department with this name already exists.");
                }

                _mapper.Map(updateDto, department);
                
                _unitOfWork.DepartmentRepository.Update(department);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.Failure($"An error occurred while updating the department: {ex.Message}");
            }
        }
    }
} 