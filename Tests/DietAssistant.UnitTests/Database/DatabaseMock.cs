using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using System;
using System.Collections.Generic;

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
                }
            },
            new User
            {
                UserId = 2,
                Email = "example1@mail.com",
                Name = "John Doe"
            }
        };

        public static List<User> Users => _users;
    }
}
