namespace DietAssistant.DataAccess.Contracts
{
    public interface IRepository<TEntity> 
        where TEntity : class
    {
        Task<TEntity> GetByIdAsync(Int32 id);

        Task SaveEntityAsync(TEntity entity);
    }
}
