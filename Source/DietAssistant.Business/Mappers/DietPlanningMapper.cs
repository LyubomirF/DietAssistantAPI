using DietAssistant.Business.Contracts.Models.DietPlanning.Responses;
using DietAssistant.Business.Contracts.Models.DietPlanning.Responses.Macros;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Domain.DietPlanning;

namespace DietAssistant.Business.Mappers
{
    internal static class DietPlanningMapper
    { 
        public static MealPlanResponse ToResponse(
            this DietPlan dietPlan,
            MealPlan mealPlan,
            IReadOnlyCollection<FoodDetails> foods)
        {
            return new MealPlanResponse
            {
                DietPlanId = dietPlan.DietPlanId,
                MealPlanId = mealPlan.MealPlanId,
                DayOfWeek = mealPlan.DayOfWeek.ToString(),
                Time = mealPlan.Time.ToString(@"hh\:mm"),
                FoodPlan = mealPlan.FoodPlans
                    .Zip(foods, (fp, f) => new { FoodPlan = fp, Food = f })
                    .Select(x =>
                        new FoodPlanResponse
                        {
                            FoodPlanId = x.FoodPlan.FoodPlanId,
                            FoodId = x.Food.FoodId,
                            FoodName = x.Food.FoodName,
                            ServingSize = x.FoodPlan.ServingSize,
                            Unit = x.FoodPlan.Unit
                        })
                    .ToList()
            };
        }

        public static DietPlanResponse ToResponse(this DietPlan dietPlan, IReadOnlyCollection<FoodDetails> foods)
        {
            return new DietPlanResponse
            {
                DietPlanId = dietPlan.DietPlanId,
                Name = dietPlan.PlanName,
                DayPlans = dietPlan.MealPlans
                    .GroupBy(x => x.DayOfWeek, (day, mealPlans) =>
                        new DayPlanResponse
                        {
                            DayOfWeek = day.ToString(),
                            Meals = mealPlans
                                .Select(mealPlan =>
                                    new SimpleMealPlanResponse
                                    {
                                        MealPlanId = mealPlan.MealPlanId,
                                        MealName = mealPlan.MealPlanName,
                                        Time = mealPlan.Time.ToString(@"hh\:mm"),
                                        FoodPlan = mealPlan.FoodPlans
                                            .Select(fp =>
                                                new FoodPlanResponse
                                                {
                                                    FoodPlanId = fp.FoodPlanId,
                                                    FoodId = fp.FoodId,
                                                    FoodName = foods.FirstOrDefault(x => x.FoodId == fp.FoodId)?.FoodName,
                                                    ServingSize = fp.ServingSize,
                                                    Unit = fp.Unit
                                                }).ToList()
                                    }).ToList()
                        }).ToList()
            };
        }

        public static DietPlanMacrosBreakdownResponse ToResponse(this DietPlan dietPlan, List<DayTotalMacros> macrosPerDay)
        {
            return new DietPlanMacrosBreakdownResponse
            {
                DietPlanId = dietPlan.DietPlanId,
                DietPlanName = dietPlan.PlanName,
                DaysMacros = macrosPerDay
                    .Select(dtm => new
                    {
                        DayMacros = dtm,
                        TotalCaloriesPerDay = dtm.MealPlansMacros.Select(x => x.TotalCalories).Aggregate((c1, c2) => c1 + c2),
                        TotalProteinPerDay = dtm.MealPlansMacros.Select(x => x.TotalProtein).Aggregate((p1, p2) => p1 + p2),
                        TotalCarbsPerDay = dtm.MealPlansMacros.Select(x => x.TotalCarbs).Aggregate((c1, c2) => c1 + c2),
                        TotalFatPerDay = dtm.MealPlansMacros.Select(x => x.TotalFat).Aggregate((f1, f2) => f1 + f2)
                    })
                    .Select(x => new DayMacros
                    {
                        Day = x.DayMacros.Day.ToString(),
                        MealPlansMacros = x.DayMacros.MealPlansMacros
                            .Select(mptm => new MealPlanMacros
                            {
                                MealPlanId = mptm.MealPlanId,
                                MealPlanName = mptm.MealPlanName,
                                PercentageOfTotalCalories = Math.Round((mptm.TotalCalories / x.TotalCaloriesPerDay) * 100, 2),
                                PercentageOfTotalProtein = Math.Round((mptm.TotalProtein / x.TotalProteinPerDay) * 100, 2),
                                PercentageOfTotalCarbs = Math.Round((mptm.TotalCarbs / x.TotalCarbsPerDay) * 100, 2),
                                PercantageOfTotalFat = Math.Round((mptm.TotalFat / x.TotalFatPerDay) * 100, 2)
                            })
                            .ToList(),
                        TotalCalories = Math.Round(x.TotalCaloriesPerDay, 2),
                        TotalProtein = Math.Round(x.TotalProteinPerDay, 2),
                        TotalCarbs = Math.Round(x.TotalCarbsPerDay, 2),
                        TotalFat = Math.Round(x.TotalFatPerDay, 2)
                    })
                    .ToList()
            };
        }
    }
}
