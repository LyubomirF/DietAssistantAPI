#pragma warning disable CS8618

namespace DietAssistant.Domain
{
    public class FoodServing
    {
        public Int32 FoodServingId { get; set; }

        public Food Food { get; set; }

        public Meal Meal { get; set; }

        public Int32 MealId { get; set; }

        public Double ServingSizeAmount { get; set; }

        public String ServingSizeUnit { get; set; }

        public Int32 NumberOfServings { get; set; }
    }
}
