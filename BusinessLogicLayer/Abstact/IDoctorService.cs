using Entity.DTOs;
using Entity.DTOs.DoctorDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstact
{
    public interface IDoctorService
    {
        Task<ServiceResponse<IEnumerable<DoctorDto>>> GetAllAsync();
        Task<ServiceResponse<DoctorDto>> GetByIdAsync(int id);
        Task<ServiceResponse<DoctorDto>> CreateAsync(DoctorCreateDto createDto);
        Task<ServiceResponse<bool>> UpdateAsync(DoctorUpdateDto updateDto);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
} 