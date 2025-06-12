using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.DatabaseFolder;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete
{
    public class DoctorRepository : Repository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ProjectMainContext context) : base(context) { }

        public async Task<Doctor?> GetDoctorWithScheduleAsync(int doctorId)
        {
            return await _dbSet
                .Include(d => d.DoctorSchedules)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task<Doctor?> GetDoctorWithAppointmentsAsync(int doctorId)
        {
            return await _dbSet
                .Include(d => d.Appointments)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(int departmentId)
        {
            return await _dbSet
                .Where(d => d.DepartmentId == departmentId && d.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(DateTime date)
        {
            return await _dbSet
                .Include(d => d.DoctorSchedules)
                .Where(d => d.IsActive && 
                           d.DoctorSchedules.Any(s => 
                               s.DayOfWeek == (int)date.DayOfWeek))
                .ToListAsync();
        }

        public async Task UpdateLicenseNumberAsync(int doctorId, string licenseNumber)
        {
            var doctor = await _dbSet.FindAsync(doctorId);
            if (doctor != null)
            {
                doctor.LicenseNumber = licenseNumber;
                doctor.ModifiedDate = DateTime.UtcNow;
            }
        }
    }
} 