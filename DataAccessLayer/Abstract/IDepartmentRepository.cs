using Entity.Models;

namespace DataAccessLayer.Abstract
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<Department?> GetDepartmentWithDoctorsAsync(int departmentId);
        Task<IEnumerable<Department>> GetActiveDepartmentsAsync();
    }
} 