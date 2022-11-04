using DietAssistant.Business.Helpers;
using DietAssistant.Domain.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.UnitTests.HelperTests
{
    using static CalorieHelper;

    public class CalorieHelperTests
    {
        [Test]
        public void ChangeWeeklyGoal_ReturnsMaintainWeight()
        {
            //Arrange
            var currentWeight = 75.5;
            var goalWeight = 75;

            //Act
            var result1 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.ModerateWeightLoss, WeightUnit.Kilograms);
            var result2 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.SlowWeightLoss, WeightUnit.Kilograms);
            var result3 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.ModerateWeightGain, WeightUnit.Kilograms);

            //Assert
            Assert.AreEqual(result1, WeeklyGoal.MaintainWeight);
            Assert.AreEqual(result2, WeeklyGoal.MaintainWeight);
            Assert.AreEqual(result3, WeeklyGoal.MaintainWeight);
        }

        [Test]
        public void ChangeWeeklyGoal_ReturnsSlowWeightGain()
        {
            //Arrange
            var currentWeight = 75.5;
            var goalWeight = 80;

            //Act
            var result1 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.ModerateWeightLoss, WeightUnit.Kilograms);
            var result2 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.SlowWeightLoss, WeightUnit.Kilograms);
            var result3 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.IntenseWeightLoss, WeightUnit.Kilograms);
            var result4 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.MaintainWeight, WeightUnit.Kilograms);

            //Assert
            Assert.AreEqual(result1, WeeklyGoal.SlowWeightGain);
            Assert.AreEqual(result2, WeeklyGoal.SlowWeightGain);
            Assert.AreEqual(result3, WeeklyGoal.SlowWeightGain);
            Assert.AreEqual(result4, WeeklyGoal.SlowWeightGain);
        }

        [Test]
        public void ChangeWeeklyGoal_ReturnsModerateWeightLoss()
        {
            //Arrange
            var currentWeight = 75.5;
            var goalWeight = 71;

            //Act
            var result1 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.SlowWeightGain, WeightUnit.Kilograms);
            var result2 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.ModerateWeightGain, WeightUnit.Kilograms);
            var result3 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.MaintainWeight, WeightUnit.Kilograms);

            //Assert
            Assert.AreEqual(result1, WeeklyGoal.ModerateWeightLoss);
            Assert.AreEqual(result2, WeeklyGoal.ModerateWeightLoss);
            Assert.AreEqual(result3, WeeklyGoal.ModerateWeightLoss);
        }

        [Test]
        public void ChangeWeeklyGoal_WeeklyGoalShouldNotChange()
        {
            //Arrange
            var currentWeight = 75.5;
            var goalWeight = 71;

            //Act
            var result1 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.SlowWeightLoss, WeightUnit.Kilograms);
            var result2 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.ModerateWeightLoss, WeightUnit.Kilograms);
            var result3 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.IntenseWeightLoss, WeightUnit.Kilograms);
            var result4 = ChangeWeeklyGoal(currentWeight, goalWeight, WeeklyGoal.ExtremeWeightLoss, WeightUnit.Kilograms);

            //Assert
            Assert.AreEqual(result1, WeeklyGoal.SlowWeightLoss);
            Assert.AreEqual(result2, WeeklyGoal.ModerateWeightLoss);
            Assert.AreEqual(result3, WeeklyGoal.IntenseWeightLoss);
            Assert.AreEqual(result4, WeeklyGoal.ExtremeWeightLoss);
        }
    }
}
