using Entity.DTOs;
using Entity.DTOs.DepartmentDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstact
{
    public interface IDepartmentService
    {
        Task<ServiceResponse<IEnumerable<DepartmentDto>>> GetAllAsync();
        Task<ServiceResponse<DepartmentDto>> GetByIdAsync(int id);
        Task<ServiceResponse<DepartmentDto>> CreateAsync(DepartmentCreateDto createDto);
        Task<ServiceResponse<bool>> UpdateAsync(DepartmentUpdateDto updateDto);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
} 