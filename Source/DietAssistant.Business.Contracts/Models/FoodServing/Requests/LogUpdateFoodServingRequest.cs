using System.ComponentModel.DataAnnotations;

namespace DietAssistant.Business.Contracts.Models.FoodServing.Requests
{
    public class LogUpdateFoodServingRequest
    {
        public String FoodId { get; set; }

        public Double ServingSize { get; set; }

        public String Unit { get; set; }

        public Double NumberOfServings { get; set; }
    }
}
