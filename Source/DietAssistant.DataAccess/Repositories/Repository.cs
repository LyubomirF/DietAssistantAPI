namespace DietAssistant.DataAccess.Repositories
{
    public abstract class Repository<TEntity>
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

        protected async Task<Int32> DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task SaveEntityAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
