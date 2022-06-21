namespace DietAssistant.Business
{
    public class ResponseMessages
    {
        public const String Unauthorized = nameof(Unauthorized);

        public static String MealNotFound(Int32 id) => $"Meal with id {id} was not found.";

        public const String CannotDeleteMeal = "Could not delete meal.";

        public static String FoodServingNotFound(Int32 id) => $"Food serving with id {id} was not found.";

        public const String CannotDeleteFoodServing = "Ccould not delete food serving.";

        public const String CannotConvertToUnit = "Cannot convert to unit.";

        public const String ActionFailed = nameof(ActionFailed);
    }
}
