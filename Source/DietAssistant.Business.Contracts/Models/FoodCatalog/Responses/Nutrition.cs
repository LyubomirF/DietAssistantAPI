#pragma warning disable CS8618


namespace DietAssistant.Business.Contracts.Models.FoodCatalog.Responses
{
    public class Nutrition
    {
        public List<Nutrient> Nutrients { get; set; }

        public double Calories { get; set; }

        public string Carbs { get; set; }

        public string Fat { get; set; }

        public string Protein { get; set; }
    }
}
