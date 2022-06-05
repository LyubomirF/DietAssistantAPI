﻿namespace DietAssistant.Business
{
    public static class NutritionApiRoutes
    {
        public const string BaseUrl = "https://spoonacular-recipe-food-nutrition-v1.p.rapidapi.com/";

        public const string SearchFoods = "food/products/search";

        public static string GetFood(Int32 id) => $"food/products/{id}";
    }
}