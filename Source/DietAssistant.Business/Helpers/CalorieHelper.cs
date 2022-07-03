using DietAssistant.Domain.Enums;

namespace DietAssistant.Business.Helpers
{
    internal class CalorieHelper
    {
        private const Double SedentaryMultiplier = 1.2;
        private const Double LightlyActiveMultiplier = 1.375;
        private const Double ActiveMultiplier = 1.55;
        private const Double VeryActiveMultiplier = 1.725;

        public static Double CalculateDailyCalories(
            Double heightCm,
            Double weightKg,
            Int32 age,
            Gender gender,
            ActivityLevel activity,
            WeeklyGoal goal)
        {
            var bmr = gender switch
            {
                Gender.Male => 13.387 * weightKg + 4.799 * heightCm - 5.677 * age + 88.362,
                Gender.Female => 9.247 * weightKg + 3.098 * heightCm - 4.33 * age + 447.539
            };

            var expenditure = activity switch
            {
                ActivityLevel.Sedentary => bmr * SedentaryMultiplier,
                ActivityLevel.LightlyActive => bmr * LightlyActiveMultiplier,
                ActivityLevel.Active => bmr * ActiveMultiplier,
                ActivityLevel.VeryActive => bmr * VeryActiveMultiplier
            };

            return goal switch
            {
                WeeklyGoal.MaintainWeight => expenditure,
                WeeklyGoal.SlowWeightLoss => expenditure - 250,
                WeeklyGoal.ModerateWeightLoss => expenditure - 500,
                WeeklyGoal.IntenseWeightLoss => expenditure - 750,
                WeeklyGoal.ExtremeWeightLoss => expenditure - 1000,
                WeeklyGoal.SlowWeightGain => expenditure + 250,
                WeeklyGoal.ModerateWeightGain => expenditure + 500
            };
        }
    }
}
