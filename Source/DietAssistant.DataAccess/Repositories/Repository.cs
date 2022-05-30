using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.DataAccess.Repositories
{
    public class Repository<TEntity>
        where TEntity : class
    {
        protected readonly DietAssistantDbContext _dbContext;

        protected Repository(DietAssistantDbContext dbContext)
            => _dbContext = dbContext;

        protected async Task<TEntity> GetByIdAsync(Int32 id)
        {
            var entity = await _dbContext
                .Set<TEntity>()
                .FindAsync(id)
                .AsTask();

            return entity;
        }

        protected async Task Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveEntityAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
