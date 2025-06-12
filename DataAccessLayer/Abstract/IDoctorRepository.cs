using Entity.Models;

namespace DataAccessLayer.Abstract
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        Task<Doctor?> GetDoctorWithScheduleAsync(int doctorId);
        Task<Doctor?> GetDoctorWithAppointmentsAsync(int doctorId);
        Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(int departmentId);
        Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(DateTime date);
        Task UpdateLicenseNumberAsync(int doctorId, string licenseNumber);
    }
} 