using DietAssistant.Business.Contracts.Models.Paging;
#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.NutritionApi.Requests
{
    public class SearchFoodRequest : PagingParameters
    {
        [Required]
        public String SearchQuery { get; set; }

        public Double? MinCalories { get; set; }

        public Double? MaxCalories { get; set; }

        public Double? MaxCarbs { get; set; }

        public Double? MinCarbs { get; set; }

        public Double? MaxFat { get; set; }

        public Double? MinFat { get; set; }

        public Double? MaxProtein { get; set; }

        public Double? MinProtein { get; set; }
    }
}
