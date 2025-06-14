using Entity.DTOs.DoctorDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstact
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDto>> GetAllAsync();
        Task<DoctorDto> GetByIdAsync(int id);
        Task<DoctorDto> CreateAsync(DoctorCreateDto createDto);
        Task UpdateAsync(DoctorUpdateDto updateDto);
        Task DeleteAsync(int id);
    }
} 