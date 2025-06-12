using Entity.Models;

namespace DataAccessLayer.Abstract
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<IEnumerable<Patient>> GetPatientsByDoctorAsync(int doctorId);
        Task UpdateLastVisitDateAsync(int patientId);
    }
} 