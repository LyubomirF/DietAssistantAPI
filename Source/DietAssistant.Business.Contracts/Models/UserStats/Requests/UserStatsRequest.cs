using DietAssistant.Domain.Enums;

namespace DietAssistant.Business.Contracts.Models.UserStats.Requests
{
    public class UserStatsRequest
    {
        public Double Height { get; set; }

        public Double Weight { get; set; }

        public String HeightUnit { get; set; }

        public String WeightUnit { get; set; }

        public String Gender { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
