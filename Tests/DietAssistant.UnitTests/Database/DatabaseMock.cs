using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable

namespace DietAssistant.UnitTests.Database
{
    internal class DatabaseMock
    {
        private static List<User> _users = new()
        {
            new User
            {
                UserId = 1,
                Email = "example@mail.com",
                Name = "John Smith",
                UserStats = new UserStats
                {
                    Gender = Gender.Male,
                    Height = 176,
                    Weight = 85,
                    HeightUnit = HeightUnit.Centimeters,
                    WeightUnit = WeightUnit.Kilograms,
                    DateOfBirth = new DateTime(1995, 1, 13)
                },
                ProgressLogs = new List<ProgressLog>
                {
                    new ProgressLog
                    {
                        ProgressLogId = 1,
                        MeasurementType = MeasurementType.Weight,
                        Measurement = 90.0,
                        LoggedOn = new DateTime(2021, 10, 1),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 2,
                        MeasurementType = MeasurementType.Weight,
                        Measurement = 90.1,
                        LoggedOn = new DateTime(2021, 10, 2),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 2,
                        MeasurementType = MeasurementType.Weight,
                        Measurement = 89.7,
                        LoggedOn = new DateTime(2021, 10, 3),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 3,
                        MeasurementType = MeasurementType.Weight,
                        Measurement = 90.5,
                        LoggedOn = new DateTime(2021, 10, 4),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 4,
                        MeasurementType = MeasurementType.Weight,
                        Measurement = 90.1,
                        LoggedOn = new DateTime(2021, 10, 5),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 5,
                        MeasurementType = MeasurementType.Weight,
                        Measurement = 90.3,
                        LoggedOn = new DateTime(2021, 10, 6),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 6,
                        MeasurementType = MeasurementType.Weight,
                        Measurement = 85,
                        LoggedOn = new DateTime(2022, 5, 1),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 7,
                        MeasurementType = MeasurementType.Waist,
                        Measurement = 85.0,
                        LoggedOn = new DateTime(2021, 10, 2),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 8,
                        MeasurementType = MeasurementType.Waist,
                        Measurement = 85.0,
                        LoggedOn = new DateTime(2021, 10, 4),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 9,
                        MeasurementType = MeasurementType.Waist,
                        Measurement = 85.0,
                        LoggedOn = new DateTime(2021, 10, 6),
                        UserId = 1
                    },
                    new ProgressLog
                    {
                        ProgressLogId = 10,
                        MeasurementType = MeasurementType.Waist,
                        Measurement = 85.0,
                        LoggedOn = new DateTime(2021, 10, 8),
                        UserId = 1
                    },
                },
                Goal = new Goal
                {
                    GoalId = 1,
                    StartDate = new DateTime(2021, 5, 1),
                    StartWeight = 95,
                    CurrentWeight = 85,
                    GoalWeight = 80,
                    WeeklyGoal = WeeklyGoal.SlowWeightLoss,
                    ActivityLevel = ActivityLevel.Sedentary,
                    UserId = 1,
                    NutritionGoal = new NutritionGoal
                    {
                        NutritionGoalId = 1,
                        Calories = 2100.0,
                        PercentProtein = 35,
                        PercentFat = 25,
                        PercentCarbs = 40,
                        ChangedOnUTC = new DateTime()
                    }
                },
                Meals = new List<Meal>()
                {
                    new Meal
                    {
                        MealId = 1,
                        Order = 1,
                        EatenOn = new DateTime(2021, 12, 1),
                        UserId = 1,
                        FoodServings = new List<FoodServing>()
                        {
                            new FoodServing
                            {
                                FoodServingId = 1,
                                FoodId = "W1111", //Chicken
                                MealId = 1,
                                ServingUnit = "g",
                                ServingSize = 120,
                                NumberOfServings = 1,
                            },
                            new FoodServing
                            {
                                FoodServingId = 2,
                                FoodId = "W4444", //Tomatoe
                                MealId = 1,
                                ServingUnit = "g",
                                ServingSize = 200,
                                NumberOfServings = 1,
                            }
                        }
                    },
                    new Meal
                    {
                        MealId = 2,
                        Order = 2,
                        EatenOn = new DateTime(2021, 12, 1),
                        UserId = 1,
                        FoodServings = new List<FoodServing>()
                        {
                            new FoodServing
                            {
                                FoodServingId = 3,
                                FoodId = "W3333", //Banana
                                MealId = 2,
                                ServingUnit = "g",
                                ServingSize = 75,
                                NumberOfServings = 1,
                            },
                        }
                    },
                    new Meal
                    {
                        MealId = 3,
                        Order = 1,
                        EatenOn = new DateTime(2021, 12, 2),
                        UserId = 1,
                        FoodServings = new List<FoodServing>()
                        {
                            new FoodServing
                            {
                                FoodServingId = 4,
                                FoodId = "W2222", //Fine Grained Oats
                                MealId = 3,
                                ServingUnit = "g",
                                ServingSize = 60,
                                NumberOfServings = 1,
                            },
                            new FoodServing
                            {
                                FoodServingId = 5,
                                FoodId = "W4444", //Tomatoe
                                MealId = 3,
                                ServingUnit = "g",
                                ServingSize = 200,
                                NumberOfServings = 1,
                            }
                        }
                    },
                    new Meal
                    {
                        MealId = 4,
                        Order = 2,
                        EatenOn = new DateTime(2021, 12, 2),
                        UserId = 1,
                        FoodServings = new List<FoodServing>()
                        {
                            new FoodServing
                            {
                                FoodServingId = 6,
                                FoodId = "W4444", //Tomatoe
                                MealId = 1,
                                ServingUnit = "g",
                                ServingSize = 220,
                                NumberOfServings = 1
                            },
                        }
                    }
                }
            },
            new User
            {
                UserId = 2,
                Email = "example1@mail.com",
                Name = "John Doe"
            },
            new User
            {
                UserId = 3,
                Email = "example2@mail.com",
                Name = "John Doe 1",
                UserStats = new UserStats
                {
                    Gender = Gender.Male,
                    Height = 70,
                    Weight = 187,
                    HeightUnit = HeightUnit.Inches,
                    WeightUnit = WeightUnit.Pounds,
                    DateOfBirth = new DateTime(1995, 1, 13)
                },
                Goal = new Goal
                {
                    GoalId = 1,
                    StartDate = new DateTime(2021, 5, 1),
                    StartWeight = 209,
                    CurrentWeight = 187,
                    GoalWeight = 176,
                    WeeklyGoal = WeeklyGoal.SlowWeightLoss,
                    ActivityLevel = ActivityLevel.Sedentary,
                    UserId = 1,
                    NutritionGoal = new NutritionGoal
                    {
                        NutritionGoalId = 1,
                        Calories = 2100.0,
                        PercentProtein = 35,
                        PercentFat = 25,
                        PercentCarbs = 40,
                        ChangedOnUTC = new DateTime()
                    }
                },
                ProgressLogs = new List<ProgressLog>()
            },

        };

        public static List<User> Users { get; private set; } 

        public static void Initialize()
        {
            var usersCopy = _users
                .Select(x => new User
                {
                    UserId = x.UserId,
                    Email = x.Email,
                    Name = x.Name,
                    UserStats = x.UserStats == null
                        ? null
                        : new UserStats
                        {
                            Gender = x.UserStats.Gender,
                            Height = x.UserStats.Height,
                            Weight = x.UserStats.Weight,
                            HeightUnit = x.UserStats.HeightUnit,
                            WeightUnit = x.UserStats.WeightUnit,
                            DateOfBirth = x.UserStats.DateOfBirth
                        },
                    ProgressLogs = x.ProgressLogs == null
                        ? null
                        : x.ProgressLogs
                            .Select(pl => new ProgressLog 
                            {
                                ProgressLogId = pl.ProgressLogId,
                                MeasurementType = pl.MeasurementType,
                                Measurement = pl.Measurement,
                                LoggedOn = pl.LoggedOn,
                                UserId = pl.UserId
                            })
                            .ToList(),
                    Goal = x.Goal == null
                        ? null
                        : new Goal
                        {
                            GoalId = x.Goal.GoalId,
                            StartDate = x.Goal.StartDate,
                            StartWeight = x.Goal.StartWeight,
                            CurrentWeight = x.Goal.CurrentWeight,
                            GoalWeight = x.Goal.GoalWeight,
                            WeeklyGoal = x.Goal.WeeklyGoal,
                            ActivityLevel = x.Goal.ActivityLevel,
                            UserId = x.Goal.UserId,
                            NutritionGoal = new NutritionGoal
                            {
                                NutritionGoalId = x.Goal.NutritionGoal.NutritionGoalId,
                                Calories = x.Goal.NutritionGoal.Calories,
                                PercentProtein = x.Goal.NutritionGoal.PercentProtein,
                                PercentFat = x.Goal.NutritionGoal.PercentFat,
                                PercentCarbs = x.Goal.NutritionGoal.PercentCarbs,
                                ChangedOnUTC = x.Goal.NutritionGoal.ChangedOnUTC
                            }
                        },
                    Meals = x.Meals == null
                        ? null
                        : x.Meals
                            .Select(m => new Meal
                            {
                                MealId = m.MealId,
                                Order = m.Order,
                                EatenOn = m.EatenOn,
                                UserId = m.UserId,
                                FoodServings = m.FoodServings == null
                                    ? null
                                    : m.FoodServings
                                        .Select(fs => new FoodServing
                                        {
                                            FoodServingId = fs.FoodServingId,
                                            FoodId = fs.FoodId,
                                            MealId = fs.MealId,
                                            ServingUnit = fs.ServingUnit,
                                            ServingSize = fs.ServingSize,
                                            NumberOfServings = fs.NumberOfServings
                                        })
                                        .ToList()
                            })
                            .ToList()
                })
                .ToList();

            Users = usersCopy;
        }
    }
}
