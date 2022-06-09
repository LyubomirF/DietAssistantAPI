namespace DietAssistant.Business.Configuration
{
    public static class NutritionApiRoutes
    {
        public const string BaseUrl = "https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/";

        public const string SearchFoods = "food/products/search";

        public static string GetFood(int id) => $"food/products/{id}";
    }
}
