namespace DietAssistant.WebAPI
{
    public static class AuthRoutes
    {
        public const String Auth = "auth";

        public const String Login = "login";

        public const String Register = "register";
    }

    public static class DiaryRoutes
    {
        public const String Diary = "diary";

        public const String Meals = "meals";

        public const String Meal = "meals/{mealId}";

        public const String FoodServings = "meals/{mealId}/food-servings";

        public const String FoodServing = "meals/{mealId}/food-servings/{foodServingId}";
    }

    public static class DietPlanningRoutes
    {
        public const String DietPlans = "diet-plans";

        public const String DietPlan = "{dietPlanId}";

        public const String Macros = "{dietPlanId}/macros";

        public const String MealPlans = "{dietPlanId}/meal-plans";

        public const String MealPlan = "{dietPlanId}/meal-plans/{mealPlanId}";

        public const String FoodPlans = "{dietPlanId}/meal-plans/{mealPlanId}/food-plans";

        public const String FoodPlan = "{dietPlanId}/meal-plans/{mealPlanId}/food-plans/{foodPlanId}";
    }

    public static class FoodCatalogRoutes
    {
        public const String Foods = "foods";

        public const String SearchFood = "search";

        public const String Food = "{id}";
    }
}
