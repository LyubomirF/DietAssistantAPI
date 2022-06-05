namespace DietAssistant.Business.NutritionApi.Responses.Food
{
    public class Nutrition
    {
        public List<Nutrient> Nutrients { get; set; }

        public Double Calories { get; set; }

        public String Carbs { get; set; }   

        public String Fat { get; set; }

        public String Protein { get; set; }
    }
}
