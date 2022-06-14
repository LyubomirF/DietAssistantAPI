using DietAssistant.Domain;

namespace DietAssistant.Business.Contracts
{
    public interface IUserResolverService
    {
        Int32? GetCurrentUserId();
    }
}
