using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.Contracts.Models.FoodServing.Requests
{
    public class LogUpdateFoodServingRequest
    {
        [Required]
        public int FoodId { get; set; }

        [Required]
        public double ServingSize { get; set; }

        [Required]
        public string Unit { get; set; }

        [Required]
        public double NumberOfServings { get; set; }
    }
}
