using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Requests
{
    public class LogFoodRequest
    {
        [Required]
        public Int32 FoodId { get; set; }

        [Required]
        public Double ServingSizeAmount { get; set; }

        [Required]
        public String ServingSizeUnit { get; set; }

        [Required]
        public Int32 NumberOfServings { get; set; }
    }
}
