using test_minimals.infra.Data;

namespace test_minimals.Repository
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
        public Task AddAsync<T>(T entity, bool autoSave = true) where T : class
        {
            _dbContext.Set<T>().Add(entity);
            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync<T>(T entity, bool autoSave = true) where T : class
        {
            _dbContext.Set<T>().Remove(entity);
            if (autoSave)
            {
                _dbContext.SaveChanges();
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            return Task.FromResult(_dbContext.Set<T>().AsEnumerable());
        }

        public Task<T?> GetByIdAsync<T>(int id) where T : class
        {
            return _dbContext.Set<T>().FindAsync(id).AsTask();
        }

        public Task SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync<T>(T entity, bool autoSave = true) where T : class
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
        Task AddAsync<T>(T entity, bool autoSave = true) where T : class;
        Task<T?> GetByIdAsync<T>(int id) where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        Task UpdateAsync<T>(T entity, bool autoSave = true) where T : class;
        Task DeleteAsync<T>(T entity, bool autoSave = true) where T : class;
        Task SaveChangesAsync();
    }
}
