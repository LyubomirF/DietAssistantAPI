namespace DietAssistant.Domain.Enums
{
    public enum WeeklyGoal
    {
        MaintainWeight = 1,
        SlowWeightLoss = 2,        // 0.5 lbs or 0.25 kgs per week
        ModerateWeightLoss = 3,    // 1 lbs or 0.5 kgs per week
        IntenseWeightLoss = 4,     // 1.5 lbs or 0.75 kgs per week
        ExtremeWeightLoss = 5,     // 2 lbs or 1 kgs per week
        SlowWeightGain = 6,        // 0.5 lbs or 0.25 kgs per week
        ModerateWeightGain = 7     // 1 lbs or 0.5 kgs per week
    }
}
