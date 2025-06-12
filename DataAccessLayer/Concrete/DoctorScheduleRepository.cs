using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.DatabaseFolder;
using Entity.Enums;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete
{
    public class DoctorScheduleRepository : Repository<DoctorSchedule>, IDoctorScheduleRepository
    {
        public DoctorScheduleRepository(ProjectMainContext context) : base(context) { }

        public async Task<IEnumerable<DoctorSchedule>> GetSchedulesByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Where(s => s.DoctorId == doctorId)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan time)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            var schedule = await _dbSet
                .FirstOrDefaultAsync(s => s.DoctorId == doctorId && 
                                        s.DayOfWeek == dayOfWeek &&
                                        s.StartTime <= time && 
                                        s.EndTime >= time);

            if (schedule == null)
                return false;

            // Check if there's any appointment at this time
            var hasAppointment = await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId &&
                              a.AppointmentDate.Date == date.Date &&
                              a.AppointmentTime == time &&
                              a.Status != AppointmentStatus.Cancelled);

            return !hasAppointment;
        }
    }
} 