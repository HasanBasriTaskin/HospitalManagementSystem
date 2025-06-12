using Entity.Models;

namespace DataAccessLayer.Abstract
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<Appointment?> GetAppointmentWithDetailsAsync(int appointmentId);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(int doctorId, DateTime date);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(int patientId);
        Task CancelAppointmentAsync(int appointmentId, string cancelledBy);
    }
} 