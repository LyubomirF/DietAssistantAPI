#pragma warning disable CS8618

namespace DietAssistant.Domain
{
    public class Food
    {
        public Int32 FoodId { get; set; }

        public String Name { get; set; }

        public String Unit { get; set; }

        public String Amount { get; set; }

        public ICollection<Nutrient> Nutrients { get; set; }
    }
}
