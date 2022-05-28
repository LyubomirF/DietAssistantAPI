#pragma warning disable CS8618

namespace DietAssistant.Domain
{
    public class Meal
    {
        public Int32 MealId { get; set; }

        public Int32 Order { get; set; }

        public DateTime EatenOn { get; set; }

        public ICollection<FoodServing> FoodServings { get; set; }
    }
}
