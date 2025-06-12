using Entity.Models;

namespace DataAccessLayer.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        // Repository Properties
        IDoctorRepository DoctorRepository { get; }
        IPatientRepository PatientRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IDoctorScheduleRepository DoctorScheduleRepository { get; }
        IDepartmentRepository DepartmentRepository { get; }

        // Transaction Operations
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        // Save Changes
        Task<int> SaveChangesAsync();
    }
} 