#pragma warning disable CS8618

namespace DietAssistant.Domain
{
    //RERUN THE MIGRATION
    public class FoodServing
    {
        public Int32 FoodServingId { get; set; }

        public Int32 FoodId { get; set; }

        public Meal Meal { get; set; }

        public Int32 MealId { get; set; }

        public String ServingUnit { get; set; }

        public Double ServingSize { get; set; }

        public Double NumberOfServings { get; set; }
    }
}
