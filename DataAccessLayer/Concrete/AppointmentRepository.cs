using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.DatabaseFolder;
using Entity.Enums;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ProjectMainContext context) : base(context) { }

        public async Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId)
        {
            return await _dbSet
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId, DateTime date)
        {
            return await _dbSet
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == doctorId && 
                           a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId)
        {
            return await _dbSet
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task CancelAppointmentAsync(int appointmentId, string cancelledBy)
        {
            var appointment = await _dbSet.FindAsync(appointmentId);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                appointment.CancelledBy = cancelledBy;
                appointment.CancelledDate = DateTime.UtcNow;
                appointment.ModifiedDate = DateTime.UtcNow;
            }
        }
    }
} 