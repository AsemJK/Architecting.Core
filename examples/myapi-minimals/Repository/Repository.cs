using Microsoft.EntityFrameworkCore;
using myapi_minimals.infra.Data;
using System.Linq.Expressions;

namespace myapi_minimals.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<Repository<T>> _logger;

        public Repository(ApplicationDbContext dbContext, ILogger<Repository<T>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public Task AddAsync(T entity, bool autoSave = true)
        {
            _dbContext.Set<T>().Add(entity);
            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity, bool autoSave = true)
        {
            _dbContext.Set<T>().Remove(entity);
            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public Task<T?> GetByIdAsync(int id)
        {
            return _dbContext.Set<T>().FindAsync(id).AsTask();
        }

        public Task SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(T entity, bool autoSave = true)
        {
            _dbContext.Set<T>().Update(entity);
            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity, bool autoSave = true);
        Task<T?> GetByIdAsync(int id);
        Task UpdateAsync(T entity, bool autoSave = true);
        Task DeleteAsync(T entity, bool autoSave = true);
        Task SaveChangesAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
    }
}
