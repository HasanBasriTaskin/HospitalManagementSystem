using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.DatabaseFolder;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete
{
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(ProjectMainContext context) : base(context) { }

        public async Task<Patient?> GetPatientWithAppointmentsAsync(int patientId)
        {
            return await _dbSet
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(int doctorId)
        {
            return await _dbSet
                .Include(p => p.Appointments)
                .Where(p => p.Appointments.Any(a => a.DoctorId == doctorId))
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Email == email && p.IsActive);
        }

        public async Task<Patient?> GetPatientByPhoneAsync(string phone)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Phone == phone && p.IsActive);
        }

        public async Task UpdateLastVisitDateAsync(int patientId)
        {
            var patient = await _dbSet.FindAsync(patientId);
            if (patient != null)
            {
                patient.LastVisitDate = DateTime.UtcNow;
                patient.ModifiedDate = DateTime.UtcNow;
            }
        }
    }
} 