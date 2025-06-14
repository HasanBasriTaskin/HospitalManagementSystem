using Entity.DTOs.PatientDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstact
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDto>> GetAllAsync();
        Task<PatientDto> GetByIdAsync(int id);
        Task<PatientDto> CreateAsync(PatientCreateDto createDto);
        Task UpdateAsync(PatientUpdateDto updateDto);
        Task DeleteAsync(int id);
    }
} 