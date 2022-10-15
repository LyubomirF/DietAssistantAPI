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

        public HeightUnit HeightUnit { get; set; }

        public WeightUnit WeightUnit { get; set; }
 
        public DateTime DateOfBirth { get; set; }

        public Double GetWeightInPounds()
            => WeightUnit == WeightUnit.Pounds 
                ? Weight
                : Math.Round(2.205 * Weight, 2);

        public Double GetWeightInKg()
            => WeightUnit == WeightUnit.Kilograms
                ? Weight
                : Math.Round(Weight / 2.205, 2);

        public Double GetHeighInInches()
            => HeightUnit == HeightUnit.FeetInches
                ? Height
                : Math.Round(Height / 2.54, 2);

        public Double GetHeighInCentimeters()
            => HeightUnit == HeightUnit.Centimeters
                ? Height
                : Math.Round(2.54 * Height, 2);
    }
}
