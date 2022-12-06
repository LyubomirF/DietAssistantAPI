using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Requests;
using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using DietAssistant.Business.Helpers;
using DietAssistant.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable

namespace DietAssistant.UnitTests.Services
{
    using static NutritionHelper;

    internal class FoodCatalogMock : IFoodCatalogService
    {
        private List<FoodDetails> _foods = new()
        {
            new FoodDetails
            {
                FoodId = "W1111",
                FoodName = "Chicken Breast",
                Nutrition = new Nutrition
                {
                    Nutrients = new List<Nutrient>
                    {
                        new Nutrient
                        {
                            Name = "Calories",
                            Amount = 160,
                            Unit = "kcal"
                        },
                        new Nutrient
                        {
                            Name = "Protein",
                            Amount = 31,
                            Unit = "g"
                        },
                        new Nutrient
                        {
                            Name = "Carbohydrates",
                            Amount = 0.2,
                            Unit = "g"
                        },
                        new Nutrient
                        {
                            Name = "Fat",
                            Amount = 3.4,
                            Unit = "g"
                        },
                    }
                },
                ServingInformation = new Serving
                {
                    Number = 1,
                    Size = 100,
                    Unit = "g"
                },
                PossibleUnits = new List<string> { "g", "oz" }
            },
            new FoodDetails
            {
                FoodId = "W2222",
                FoodName = "Fine grained Oats",
                Nutrition = new Nutrition
                {
                    Nutrients = new List<Nutrient>
                    {
                        new Nutrient
                        {
                            Name = "Calories",
                            Amount = 360,
                            Unit = "kcal"
                        },
                        new Nutrient
                        {
                            Name = "Protein",
                            Amount = 13,
                            Unit = "g"
                        },
                        new Nutrient
                        {
                            Name = "Carbohydrates",
                            Amount = 65,
                            Unit = "g"
                        },
                        new Nutrient
                        {
                            Name = "Fat",
                            Amount = 9,
                            Unit = "g"
                        },
                    }
                },
                ServingInformation = new Serving
                {
                    Number = 1,
                    Size = 100,
                    Unit = "g"
                },
                PossibleUnits = new List<string> { "g", "oz" }
            },
            new FoodDetails
            {
                FoodId = "W3333",
                FoodName = "Banana",
                Nutrition = new Nutrition
                {
                    Nutrients = new List<Nutrient>
                    {
                        new Nutrient
                        {
                            Name = "Calories",
                            Amount = 75,
                            Unit = "kcal"
                        },
                        new Nutrient
                        {
                            Name = "Protein",
                            Amount = 1,
                            Unit = "g"
                        },
                        new Nutrient
                        {
                            Name = "Carbohydrates",
                            Amount = 14,
                            Unit = "g"
                        },
                        new Nutrient
                        {
                            Name = "Fat",
                            Amount = 2,
                            Unit = "g"
                        },
                    }
                },
                ServingInformation = new Serving
                {
                    Number = 1,
                    Size = 100,
                    Unit = "g"
                },
                PossibleUnits = new List<string> { "g", "oz" },
            },
            new FoodDetails
            {
                FoodId = "W4444",
                FoodName = "Tomatoe",
                Nutrition = new Nutrition
                {
                    Nutrients = new List<Nutrient>
                    {
                        new Nutrient
                        {
                            Name = "Calories",
                            Amount = 34,
                            Unit = "kcal"
                        },
                        new Nutrient
                        {
                            Name = "Protein",
                            Amount = 1,
                            Unit = "g"
                        },
                        new Nutrient
                        {
                            Name = "Carbohydrates",
                            Amount = 6,
                            Unit = "g"
                        },
                        new Nutrient
                        {
                            Name = "Fat",
                            Amount = 0.1,
                            Unit = "g"
                        },
                    }
                },
                ServingInformation = new Serving
                {
                    Number = 1,
                    Size = 100,
                    Unit = "g"
                },
                PossibleUnits = new List<string> { "g", "oz" },
            }
        };

        public Task<Result<FoodDetails>> GetFoodByIdAsync(String id, ServingRequest request) 
            => Task.FromResult(Result.Create(GetWithRecalculatedNutrition(id, request.Amount.Value, request.Unit)));
        

        public Task<Result<IReadOnlyCollection<FoodDetails>>> GetFoodsAsync(IEnumerable<ManyFoodDetailsRequest> requests)
        {
            var foods = requests
                .Select(x => GetWithRecalculatedNutrition(x.FoodId, x.Serving.Amount.Value, x.Serving.Unit))
                .ToList();

            return Task.FromResult(Result.Create<IReadOnlyCollection<FoodDetails>>(foods));
        }

        public Task<Result<FoodSearch>> SearchFoodsAsync(SearchFoodRequest requestModel)
        {
            throw new NotImplementedException();
        }

        private FoodDetails GetWithRecalculatedNutrition(String id, Double targetAmount, String targetUnit)
        {
            var food = _foods.SingleOrDefault(x => x.FoodId == id);
            var unit = food.ServingInformation.Unit;
            var size = food.ServingInformation.Size;

            var recalculated = food.CalculateNutrition(size, unit, targetAmount, targetUnit);

            return recalculated;
        }
    }
}
