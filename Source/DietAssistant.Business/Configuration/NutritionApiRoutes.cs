namespace DietAssistant.Business.Configuration
{
    public static class NutritionApiRoutes
    {
        public const string BaseUrl = "https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/";

        public const string SearchProducts = "food/products/search";

        public const string SearchIngredients = "food/ingredients/search";

        public static string GetProduct(int id) => $"food/products/{id}";

        public static string GetIngredient(int id) => $"food/ingredients/{id}"; 
    }
}
