#pragma warning disable CS8618

namespace DietAssistant.Domain
{
    public class UserStats
    {
        public Int32 UserStatsId { get; set; }

        public User User { get; set; }

        public Int32 UserId { get; set; }

        public Double Height { get; set; }

        public Double Weight { get; set; }

        public String MetricSystem { get; set; }

        public Double? BodyFatPercentage { get; set; }
    }
}
