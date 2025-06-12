using System.Linq.Expressions;
using Entity.Models;

namespace DataAccessLayer.Abstract
{
    public interface IRepository<T> where T : class
    {
        // Async Get Operations (Database I/O operations)
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        
        // Sync Add Operations (In-memory operations)
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        
        // Sync Update Operations (In-memory operations)
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        
        // Sync Delete Operations (In-memory operations)
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        
        // Async Count Operations (Database I/O operations)
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        
        // Async Exists Operations (Database I/O operations)
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }
} 