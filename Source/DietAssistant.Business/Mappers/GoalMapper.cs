using DietAssistant.Business.Contracts.Models.Goal.Responses;
using DietAssistant.Domain;

namespace DietAssistant.Business.Mappers
{
    internal static class GoalMapper
    {
        public static GoalResponse ToResponse(this Goal goal)
        {
            return new GoalResponse
            {
                StartWeight = goal.StartWeight,
                StartDate = goal.StartDate,
                CurrentWeight = goal.CurrentWeight,
                GoalWeight = goal.GoalWeight,
                WeeklyGoal = goal.WeeklyGoal.ToString(),
                ActivityLevel = goal.ActivityLevel.ToString(),
                NutritionGoal = new Contracts.Models.Goal.Responses.NutritionGoal
                {
                    Calories = goal.NutritionGoal.Calories,
                    PercentCarbs = goal.NutritionGoal.PercentCarbs,
                    PercentProtein = goal.NutritionGoal.PercentProtein,
                    PercentFat = goal.NutritionGoal.PercentFat
                }
            };
        }

    }
}
