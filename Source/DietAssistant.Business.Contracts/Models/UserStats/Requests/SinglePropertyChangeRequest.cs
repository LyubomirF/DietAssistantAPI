using DietAssistant.Domain.Enums;

namespace DietAssistant.Business.Contracts.Models.UserStats.Requests
{
    public class ChangeHeightUnitRequest
    {
        public String HeightUnit { get; set; }
    }

    public class ChangeWeightUnitRequest
    {
        public String WeightUnit { get; set; }
    }

    public class ChangeHeightRequest
    {
        public Double Height { get; set; }
    }

    public class ChangeWeightRequest
    {
        public Double Weight { get; set; }
    }

    public class ChangeGenderRequest
    {
        public String Gender { get; set; }
    }

    public class ChangeDateOfBirthRequest
    {
        public DateTime DateOfBirth { get; set; }
    }
}
