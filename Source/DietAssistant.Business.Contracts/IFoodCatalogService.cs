using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Common;

namespace DietAssistant.Business.Contracts
{
    public interface IFoodCatalogService
    {
        Task<Result<FoodDetails>> GetFoodByIdAsync(String id, ServingRequest request);

        Task<Result<FoodSearch>> SearchFoodsAsync(SearchFoodRequest requestModel);

        Task<Result<IReadOnlyCollection<FoodDetails>>> GetFoodsAsync(IEnumerable<ManyFoodDetailsRequest> requests);
    }
}
