#pragma warning disable CS8618

namespace DietAssistant.Business.Contracts.Models.FoodCatalog.Responses
{
    public class Food
    {
        public Int32 FoodId { get; set; }

        public String FoodName { get; set; }

        public String ImagePath { get; set; }
    }
}
