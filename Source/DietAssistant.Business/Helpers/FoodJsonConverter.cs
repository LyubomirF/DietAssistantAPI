using DietAssistant.Business.Contracts.Models.FoodCatalog.Responses;
using Newtonsoft.Json;

namespace DietAssistant.Business.Helpers
{
    public static class FoodJsonConverter
    {
        // FoodId is formed with prefix W - whole food, P - product
        public static FoodSearch? DeserializeJsonProducts(string json)
        {
            var definition = new
            {
                Products = new[]
                {
                    new
                    {
                        Id = 1,
                        Title = "",
                        Image = ""
                    }
                },
                Offset = 1,
                Number = 1,
                TotalProducts = 1
            };

            var foods = JsonConvert.DeserializeAnonymousType(json, definition);

            return foods is null
                ? null
                : new FoodSearch
                {
                    Foods = foods.Products.Select(x => new Food
                    {
                        FoodId = "P" + x.Id,
                        FoodName = x.Title,
                        ImagePath = x.Image
                    })
                    .ToList(),
                    Page = foods.Offset,
                    PageSize = foods.Number,
                    TotalFoods = foods.TotalProducts,
                };
        }

        public static FoodSearch? DeserializeJsonWholeFoods(string json)
        {
            var definition = new
            {
                Results = new[]
                {
                    new
                    {
                        Id = 1,
                        Name = "",
                        Image = ""
                    }
                },
                Offset = 1,
                Number = 1,
                TotalResults = 1
            };

            var foods = JsonConvert.DeserializeAnonymousType(json, definition);

            return foods is null
                ? null
                : new FoodSearch
                {
                    Foods = foods.Results.Select(x => new Food
                    {
                        FoodId = "W" + x.Id,
                        FoodName = x.Name,
                        ImagePath = x.Image
                    })
                    .ToList(),
                    Page = foods.Offset,
                    PageSize = foods.Number,
                    TotalFoods = foods.TotalResults,
                };
        }

        public static FoodDetails? ParseProductJson(string json)
        {
            var definition = new
            {
                Id = 1,
                Title = "",
                Description = "",
                Image = "",
                Servings = new
                {
                    Number = new Nullable<Double>(1.0),
                    Size = new Nullable<Double>(1.0),
                    Unit = ""
                },
                Nutrition = new
                {
                    Nutrients = new[]
                    {
                        new {
                            Name = "",
                            Amount = new Nullable<Double>(1.0),
                            Unit = ""
                        }
                    }
                }
            };

            var food = JsonConvert.DeserializeAnonymousType(json, definition,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include
                });

            return food is null
                ? null
                : new FoodDetails
                {
                    FoodId = "P" + food.Id,
                    FoodName = food.Title,
                    ImagePath = food.Image,
                    Nutrition = new Nutrition
                    {
                        Nutrients = food.Nutrition.Nutrients
                            .Select(x => new Nutrient
                            {
                                Amount = x.Amount ?? 0,
                                Name = x.Name,
                                Unit = x.Unit,
                            })
                            .ToList(),
                    },
                    ServingInformation = new Serving
                    {
                        Number = food.Servings.Number ?? 0,
                        Unit = food.Servings.Unit,
                        Size = food.Servings.Size ?? 0
                    },
                    PossibleUnits = food.Servings.Unit == "g" || food.Servings.Unit == "oz"
                        ? new List<string> { "g", "oz" }
                        : new List<string> { food.Servings.Unit }
                };
        }

        public static FoodDetails? ParseWholeFoodJson(string json)
        {
            var definition = new
            {
                Id = 1,
                Name = "",
                Image = "",
                Amount = 1.0,
                UnitShort = "",
                Nutrition = new
                {
                    Nutrients = new[]
                       {
                        new {
                            Name = "",
                            Amount = new Nullable<Double>(1.0),
                            Unit = ""
                        }
                    },
                },
                PossibleUnits = new[] { "" }
            };

            var food = JsonConvert.DeserializeAnonymousType(json, definition,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include
                });

            return food is null
                ? null
                : new FoodDetails
                {
                    FoodId = "W" + food.Id,
                    FoodName = food.Name,
                    ImagePath = food.Image,
                    Nutrition = new Nutrition
                    {
                        Nutrients = food.Nutrition.Nutrients
                    .Select(x => new Nutrient
                    {
                        Amount = x.Amount ?? 0,
                        Name = x.Name,
                        Unit = x.Unit,
                    })
                    .ToList(),
                    },
                    ServingInformation = new Serving
                    {
                        Number = 1,
                        Unit = food.UnitShort,
                        Size = food.Amount
                    },
                    PossibleUnits = food.PossibleUnits.ToList()
                };
        }
    }
}
