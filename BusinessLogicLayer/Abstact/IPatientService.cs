using Entity.DTOs.Common;
using Entity.DTOs.PatientDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstact
{
    public interface IPatientService
    {
        Task<ServiceResponse<IEnumerable<PatientDto>>> GetAllAsync();
        Task<ServiceResponse<PatientDto>> GetByIdAsync(int id);
        Task<ServiceResponse<PatientDto>> CreateAsync(PatientCreateDto createDto);
        Task<ServiceResponse<bool>> UpdateAsync(PatientUpdateDto updateDto);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
} 