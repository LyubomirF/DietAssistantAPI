#pragma warning disable CS8618

using DietAssistant.Business.Contracts.Models.Paging;
using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.Contracts.Models.FoodCatalog.Requests
{
    public class SearchFoodRequest : PagingParameters
    {
        [Required]
        public string SearchQuery { get; set; }

        public FoodType FoodType { get; set; } = FoodType.Product;
    }

    public enum FoodType
    {
        Product = 1,
        WholeFood = 2,
    }
}
