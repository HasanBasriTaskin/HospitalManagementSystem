using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.DatabaseFolder;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ProjectMainContext context) : base(context) { }

        public async Task<Department?> GetDepartmentWithDoctorsAsync(int departmentId)
        {
            return await _dbSet
                .Include(d => d.Doctors)
                .FirstOrDefaultAsync(d => d.Id == departmentId);
        }

        public async Task<IEnumerable<Department>> GetActiveDepartmentsAsync()
        {
            return await _dbSet
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
    }
} 