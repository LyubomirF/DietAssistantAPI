using DietAssistant.Domain;

namespace DietAssistant.Business.Contracts
{
    public interface IUserResolverService
    {
        Task<User> GetCurrentUserAsync();
    }
}
