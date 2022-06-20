using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.Contracts.Models.FoodServing.Requests
{
    public class LogUpdateFoodServingRequest
    {
        [Required]
        public String FoodId { get; set; }

        [Required]
        public Double ServingSize { get; set; }

        [Required]
        public String Unit { get; set; }

        [Required]
        public Double NumberOfServings { get; set; }
    }
}
