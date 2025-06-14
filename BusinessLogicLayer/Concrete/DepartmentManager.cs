using BusinessLogicLayer.Abstact;
using DataAccessLayer.Abstract;
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

        public DepartmentManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DepartmentDto> CreateAsync(DepartmentCreateDto createDto)
        {
            var isExists = await _unitOfWork.DepartmentRepository.ExistsAsync(d => d.Name.ToLower() == createDto.Name.ToLower());
            if (isExists)
            {
                throw new InvalidOperationException("A department with this name already exists.");
            }

            var department = new Department
            {
                Name = createDto.Name,
                Description = createDto.Description,
            };

            _unitOfWork.DepartmentRepository.Add(department);
            await _unitOfWork.SaveChangesAsync();

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }

            var hasDoctors = await _unitOfWork.DoctorRepository.ExistsAsync(d => d.DepartmentId == id);
            if (hasDoctors)
            {
                throw new InvalidOperationException("This department cannot be deleted because it has doctors assigned to it.");
            }

            _unitOfWork.DepartmentRepository.Delete(department);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            return departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            });
        }

        public async Task<DepartmentDto> GetByIdAsync(int id)
        {
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {id} not found.");
            }

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };
        }

        public async Task UpdateAsync(DepartmentUpdateDto updateDto)
        {
            var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(updateDto.Id);
            if (department == null)
            {
                throw new KeyNotFoundException($"Department with ID {updateDto.Id} not found.");
            }

            var isExists = await _unitOfWork.DepartmentRepository.ExistsAsync(d => d.Name.ToLower() == updateDto.Name.ToLower() && d.Id != updateDto.Id);
            if (isExists)
            {
                throw new InvalidOperationException("A department with this name already exists.");
            }

            department.Name = updateDto.Name;
            department.Description = updateDto.Description;
            
            _unitOfWork.DepartmentRepository.Update(department);
            await _unitOfWork.SaveChangesAsync();
        }
    }
} 