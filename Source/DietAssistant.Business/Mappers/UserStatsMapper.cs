using DietAssistant.Business.Contracts.Models.UserStats.Responses;
using DietAssistant.Domain;

namespace DietAssistant.Business.Mappers
{
    internal static class UserStatsMapper
    {
        public static UserStatsResponse ToResponse(this UserStats userStats)
        {
            return new UserStatsResponse
            {
                Height = userStats.Height,
                HeightUnit = userStats.HeightUnit.ToString(),
                Weight = userStats.Weight,
                WeightUnit = userStats.WeightUnit.ToString(),
                Gender = userStats.Gender.ToString(),
                DateOfBirth = userStats.DateOfBirth.ToString("dd MMMM yyyy")
            };
        }
    }
}
