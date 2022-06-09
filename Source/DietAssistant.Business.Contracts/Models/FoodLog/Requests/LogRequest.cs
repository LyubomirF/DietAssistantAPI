using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.Contracts.Models.FoodLog.Requests
{
    public class LogRequest
    {
        [Required]
        public Int32 FoodId { get; set; }

        [Required]
        public Int32 MealId { get; set; }

        [Required]
        public Double ServingSizeAmount { get; set; }

        [Required]
        public String ServingSizeUnit { get; set; }

        [Required]
        public Int32 NumberOfServings { get; set; }
    }
}
