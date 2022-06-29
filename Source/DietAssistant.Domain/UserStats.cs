#pragma warning disable CS8618

using DietAssistant.Domain.Enums;

namespace DietAssistant.Domain
{
    public class UserStats
    {
        public Int32 UserStatsId { get; set; }

        public User User { get; set; }

        public Int32 UserId { get; set; }

        public Gender Gender { get; set; }

        public Double Height { get; set; }

        public Double Weight { get; set; }

        public HeightUnits HeightUnit { get; set; }

        public WeightUnits WeightUnit { get; set; }
 
        public DateTime DateOfBirth { get; set; }
    }
}
