using Entity.DTOs.DoctorScheduleDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Abstact
{
    public interface IDoctorScheduleService
    {
        Task<IEnumerable<DoctorScheduleDto>> GetAllByDoctorIdAsync(int doctorId);
        Task<DoctorScheduleDto> GetByIdAsync(int id);
        Task<DoctorScheduleDto> CreateAsync(DoctorScheduleCreateDto createDto);
        Task UpdateAsync(DoctorScheduleUpdateDto updateDto);
        Task DeleteAsync(int id);
    }
} 