using Entity.DTOs;
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
        Task<ServiceResponse<IEnumerable<DoctorScheduleDto>>> GetAllByDoctorIdAsync(int doctorId);
        Task<ServiceResponse<DoctorScheduleDto>> GetByIdAsync(int id);
        Task<ServiceResponse<DoctorScheduleDto>> CreateAsync(DoctorScheduleCreateDto createDto);
        Task<ServiceResponse<bool>> UpdateAsync(DoctorScheduleUpdateDto updateDto);
        Task<ServiceResponse<bool>> DeleteAsync(int id);
    }
} 