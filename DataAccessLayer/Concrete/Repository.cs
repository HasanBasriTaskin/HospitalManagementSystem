using System.Linq.Expressions;
using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete.DatabaseFolder;
using Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Concrete
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ProjectMainContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ProjectMainContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // Async Get Operations (Database I/O operations)
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        // Sync Add Operations (In-memory operations)
        public virtual void Add(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.CreatedDate = DateTime.UtcNow;
                baseEntity.IsActive = true;
            }
            _dbSet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.CreatedDate = DateTime.UtcNow;
                    baseEntity.IsActive = true;
                }
            }
            _dbSet.AddRange(entities);
        }

        // Sync Update Operations (In-memory operations)
        public virtual void Update(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.ModifiedDate = DateTime.UtcNow;
            }
            _dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.ModifiedDate = DateTime.UtcNow;
                }
            }
            _dbSet.UpdateRange(entities);
        }

        // Sync Delete Operations (In-memory operations)
        public virtual void Delete(T entity)
        {
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsActive = false;
                baseEntity.ModifiedDate = DateTime.UtcNow;
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.IsActive = false;
                    baseEntity.ModifiedDate = DateTime.UtcNow;
                }
                else
                {
                    _dbSet.Remove(entity);
                }
            }
        }

        // Async Count Operations (Database I/O operations)
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();
            return await _dbSet.CountAsync(predicate);
        }

        // Async Exists Operations (Database I/O operations)
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
} 