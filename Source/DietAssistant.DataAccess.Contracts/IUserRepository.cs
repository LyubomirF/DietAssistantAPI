using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByEmailAsync(String email);
    }
}
