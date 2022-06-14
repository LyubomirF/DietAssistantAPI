using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Requests
{
    public class LogFoodRequest
    {
        [Required]
        public Int32 FoodId { get; set; }

        [Required]
        public Double ServingSize { get; set; }

        [Required]
        public String Unit { get; set; }

        [Required]
        public Double NumberOfServings { get; set; }
    }
}
