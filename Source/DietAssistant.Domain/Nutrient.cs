#pragma warning disable CS8618

namespace DietAssistant.Domain
{
    public class Nutrient
    {
        public Int32 NutrientId { get; set; }

        public String Name { get; set; }

        public String Unit { get; set; }

        public Double Amount { get; set; }

        public Food Food { get; set; }

        public Int32 FoodId { get; set; }
    }
}
