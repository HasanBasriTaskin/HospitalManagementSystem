using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.DatabaseFolder;
using Entity.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataAccessLayer.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProjectMainContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        // Repository instances
        private IDoctorRepository? _doctorRepository;
        private IPatientRepository? _patientRepository;
        private IAppointmentRepository? _appointmentRepository;
        private IDoctorScheduleRepository? _doctorScheduleRepository;
        private IDepartmentRepository? _departmentRepository;
        private IUserRefreshTokenRepository _userRefreshTokenRepository;

        public UnitOfWork(ProjectMainContext context)
        {
            _context = context;
        }

        // Repository Properties
        public IDoctorRepository DoctorRepository => 
            _doctorRepository ??= new DoctorRepository(_context);

        public IPatientRepository PatientRepository => 
            _patientRepository ??= new PatientRepository(_context);

        public IAppointmentRepository AppointmentRepository => 
            _appointmentRepository ??= new AppointmentRepository(_context);

        public IDoctorScheduleRepository DoctorScheduleRepository => 
            _doctorScheduleRepository ??= new DoctorScheduleRepository(_context);

        public IDepartmentRepository DepartmentRepository => 
            _departmentRepository ??= new DepartmentRepository(_context);

        public IUserRefreshTokenRepository UserRefreshTokenRepository => _userRefreshTokenRepository ??= new UserRefreshTokenRepository(_context);

        // Transaction Operations
        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        // Save Changes
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Dispose Pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
                if (_transaction != null)
                {
                    _transaction.Dispose();
                }
            }
            _disposed = true;
        }
    }
} 