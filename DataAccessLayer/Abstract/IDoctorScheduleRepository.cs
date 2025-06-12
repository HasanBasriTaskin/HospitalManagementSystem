using Entity.Models;

namespace DataAccessLayer.Abstract
{
    public interface IDoctorScheduleRepository : IRepository<DoctorSchedule>
    {
        Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorAsync(int doctorId);
        Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan time);
    }
} 