namespace DietAssistant.Business.Contracts.Models.UserStats.Responses
{
    public class UserStatsResponse
    {
        public Double Height { get; set; }

        public String HeightUnit { get; set; }

        public Double Weight { get; set; }

        public String WeightUnit { get; set; }

        public String Gender { get; set; }

        public String DateOfBirth { get; set; }
    }
}
