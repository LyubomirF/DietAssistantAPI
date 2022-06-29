using DietAssistant.Domain.Enums;

namespace DietAssistant.Business.Contracts.Models.UserStats.Requests
{
    public class UserStatsRequest
    {
        public Double Height { get; set; }

        public Double Weight { get; set; }

        public HeightUnits HeightUnit { get; set; }

        public WeightUnits WeightUnit { get; set; }

        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

    }
}
