#pragma warning disable CS8618

using DietAssistant.Business.Contracts.Models.Paging;
using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.Contracts.Models.FoodCatalog.Requests
{
    public class SearchFoodRequest : PagingParameters
    {
        [Required]
        public string SearchQuery { get; set; }

        public double? MinCalories { get; set; }

        public double? MaxCalories { get; set; }

        public double? MaxCarbs { get; set; }

        public double? MinCarbs { get; set; }

        public double? MaxFat { get; set; }

        public double? MinFat { get; set; }

        public double? MaxProtein { get; set; }

        public double? MinProtein { get; set; }
    }
}
