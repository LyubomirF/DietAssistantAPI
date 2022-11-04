using DietAssistant.Business;
using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Goal.Requests;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.UnitTests.Database;
using DietAssistant.UnitTests.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable

namespace DietAssistant.UnitTests
{
    using static DatabaseMock;

    public class GoalServiceTests
    {
        private IUserRepository _userRepository;

        private Mock<IUserResolverService> _userResolverServiceMock;

        [SetUp]
        public void Setup()
        {

            DatabaseMock.Initialize();
            _userRepository = new UserRepositoryMock();
            _userResolverServiceMock = new Mock<IUserResolverService>();
        }

        [Test]
        public async Task GetGoalAsync_ReturnsSuccess() 
        {
            //Arrange
            var userId = 1;

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.GetGoalAsync();

            //Assert
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.ActivityLevel == "Sedentary"
                && data.CurrentWeight == 85
                && data.WeeklyGoal == "SlowWeightLoss"
                && data.NutritionGoal.Calories == 2100);
        }

        [Test]
        public async Task GetGoalAsync_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.GetGoalAsync();

            //Assert
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task GetGoalAsync_ReturnsUnauthorized()
        {
            //Arrange
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.GetGoalAsync();

            //Assert
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeCurrentWeightAsync_ReturnsSuccess_DoesNotChangeWeeklyGoal()
        {
            //Arrange
            var userId = 1;

            var request = new ChangeCurrentWeighRequest
            {
                CurrentWeight = 84.5
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            var user = GetUserById(userId);
            var logsCount = user.ProgressLogs.Count;

            //Act
            var result = await goalService.ChangeCurrentWeightAsync(request);

            //Assert
            var data = result.Data;
            var updatedUser = GetUserById(userId);
            var updatedStats = updatedUser.UserStats;
            var updatedLogsCount = updatedUser.ProgressLogs.Count;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.CurrentWeight == 84.5);
            Assert.IsTrue(data.WeeklyGoal == "SlowWeightLoss");
            Assert.IsTrue(logsCount + 1 == updatedLogsCount);
            Assert.IsTrue(updatedStats.Weight == 84.5);
            Assert.IsTrue(data.NutritionGoal.Calories != 2100.0);
        }

        [Test]
        public async Task ChangeCurrentWeightAsync_ReturnsSuccess_ChangesWeeklyGoal()
        {
            //Arrange
            var userId = 1;

            var request = new ChangeCurrentWeighRequest
            {
                CurrentWeight = 80.4
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            var user = GetUserById(userId);
            var logsCount = user.ProgressLogs.Count;

            //Act
            var result = await goalService.ChangeCurrentWeightAsync(request);

            //Assert
            var data = result.Data;
            var updatedUser = GetUserById(userId);
            var updatedStats = updatedUser.UserStats;
            var updatedLogsCount = updatedUser.ProgressLogs.Count;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.CurrentWeight == 80.4);
            Assert.IsTrue(data.WeeklyGoal == "MaintainWeight");
            Assert.IsTrue(logsCount + 1 == updatedLogsCount);
            Assert.IsTrue(updatedStats.Weight == 80.4);
            Assert.IsTrue(data.NutritionGoal.Calories != 2100.0);
        }

        [Test]
        public async Task ChangeCurrentWeightAsync_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;

            var request = new ChangeCurrentWeighRequest
            {
                CurrentWeight = 85
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            var user = GetUserById(userId);

            //Act
            var result = await goalService.ChangeCurrentWeightAsync(request);

            //Assert

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task ChangeCurrentWeightAsync_ReturnsUnauthorized()
        {
            //Arrange
            var userId = 2;

            var request = new ChangeCurrentWeighRequest
            {
                CurrentWeight = 85
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            var user = GetUserById(userId);

            //Act
            var result = await goalService.ChangeCurrentWeightAsync(request);

            //Assert

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeGoalWeightAsync_ReturnsSuccess_DoesNotChangeWeeklyGoal()
        {
            //Arrange
            var userId = 1;

            var request = new ChangeGoalWeightRequest
            {
                GoalWeight = 75
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            var user = GetUserById(userId);
            var logsCount = user.ProgressLogs.Count;

            //Act
            var result = await goalService.ChangeGoalWeightAsync(request);

            //Assert
            var data = result.Data;
            var updatedUser = GetUserById(userId);
            var updatedStats = updatedUser.UserStats;
            var updatedLogsCount = updatedUser.ProgressLogs.Count;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.GoalWeight == 75);
            Assert.IsTrue(data.WeeklyGoal == "SlowWeightLoss");
            Assert.IsTrue(data.NutritionGoal.Calories == 2100.0);
        }

        [Test]
        public async Task ChangeGoalWeightAsync_ReturnsSuccess_ChangesWeeklyGoal()
        {
            //Arrange
            var userId = 1;

            var request = new ChangeGoalWeightRequest
            {
                GoalWeight = 85
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            var user = GetUserById(userId);
            var logsCount = user.ProgressLogs.Count;

            //Act
            var result = await goalService.ChangeGoalWeightAsync(request);

            //Assert
            var data = result.Data;
            var updatedUser = GetUserById(userId);
            var updatedStats = updatedUser.UserStats;
            var updatedLogsCount = updatedUser.ProgressLogs.Count;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.GoalWeight == 85);
            Assert.IsTrue(data.WeeklyGoal == "MaintainWeight");
            Assert.IsTrue(data.NutritionGoal.Calories != 2100.0);
        }

        [Test]
        public async Task ChangeGoalWeightAsync_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;

            var request = new ChangeGoalWeightRequest
            {
                GoalWeight = 85
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeGoalWeightAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task ChangeGoalWeightAsync_ReturnsUnauthorized()
        {
            //Arrange
            var request = new ChangeGoalWeightRequest
            {
                GoalWeight = 85
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeGoalWeightAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeWeeklyGoalAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeWeeklyGoalRequest
            {
                WeeklyGoal = "ModerateWeightLoss"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeWeeklyGoalAsync(request);

            //Assert
            var data = result.Data;
            var updatedUser = GetUserById(userId);
            var updatedGoal = updatedUser.Goal;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.WeeklyGoal == "ModerateWeightLoss");
            Assert.IsTrue(updatedGoal.NutritionGoal.Calories != 2100.0);
        }

        [Test]
        public async Task ChangeWeeklyGoalAsync_ReturnsSuccess_WeeklyGoalNotChanged()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeWeeklyGoalRequest
            {
                WeeklyGoal = "SlowWeightLoss"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeWeeklyGoalAsync(request);

            //Assert
            var data = result.Data;
            var updatedUser = GetUserById(userId);
            var updatedGoal = updatedUser.Goal;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.WeeklyGoal == "SlowWeightLoss");
            Assert.IsTrue(updatedGoal.NutritionGoal.Calories == 2100.0);
        }


        [Test]
        public async Task ChangeWeeklyGoalAsync_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;
            var request = new ChangeWeeklyGoalRequest
            {
                WeeklyGoal = "SlowWeightLoss"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeWeeklyGoalAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task ChangeWeeklyGoalAsync_ReturnsUnauthorized()
        {
            //Arrange
            var request = new ChangeWeeklyGoalRequest
            {
                WeeklyGoal = "SlowWeightLoss"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeWeeklyGoalAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }


        [Test]
        public async Task ChangeWeeklyGoalAsync_ReturnsInvalidParameters_InvalidWeeklyGoal()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeWeeklyGoalRequest
            {
                WeeklyGoal = "WeightLoss"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeWeeklyGoalAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
            Assert.IsTrue(result.Errors.Contains("Invalid weekly goal value."));
        }

        [Test]
        public async Task ChangeActivityLevelAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeActivityLevelRequest
            {
                ActivityLevel = "Active"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeActivityLevelAsync(request);

            //Assert
            var data = result.Data;
            var updatedUser = GetUserById(userId);
            var updatedGoal = updatedUser.Goal;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.ActivityLevel == "Active");
            Assert.IsTrue(updatedGoal.NutritionGoal.Calories != 2100.0);
        }

        [Test]
        public async Task ChangeActivityLevelAsync_ReturnsSuccess_ActivityLevelNotChanged()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeActivityLevelRequest
            {
                ActivityLevel = "Sedentary"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeActivityLevelAsync(request);

            //Assert
            var data = result.Data;
            var updatedUser = GetUserById(userId);
            var updatedGoal = updatedUser.Goal;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.ActivityLevel == "Sedentary");
            Assert.IsTrue(updatedGoal.NutritionGoal.Calories == 2100.0);
        }


        [Test]
        public async Task ChangeActivityLevelAsync_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;
            var request = new ChangeActivityLevelRequest
            {
                ActivityLevel = "Sedentary"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeActivityLevelAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task ChangeActivityLevelAsync_ReturnsUnauthorized()
        {
            //Arrange
            var request = new ChangeActivityLevelRequest
            {
                ActivityLevel = "Sedentary"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeActivityLevelAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeActivityLevel_ReturnsInvalidParameters_InvalidActivityLevel()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeActivityLevelRequest
            {
                ActivityLevel = "ExtremelyActive"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeActivityLevelAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
            Assert.IsTrue(result.Errors.Contains("Invalid activity level value."));
        }

        [Test]
        public async Task ChangeNutritionGoalAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;

            var request = new NutritionGoalRequest
            {
                Calories = 2000,
                PercentCarbs = 20,
                PercentProtein = 50,
                PercentFat = 30,
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeNutritionGoalAsync(request);

            //Assert
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.NutritionGoal.Calories == 2000);
            Assert.IsTrue(data.NutritionGoal.PercentCarbs == 20);
            Assert.IsTrue(data.NutritionGoal.PercentProtein == 50);
            Assert.IsTrue(data.NutritionGoal.PercentFat == 30);
        }

        [Test]
        public async Task ChangeNutritionGoalAsync_ReturnsNotfound()
        {
            //Arrange
            var userId = 2;

            var request = new NutritionGoalRequest
            {
                Calories = 2000,
                PercentCarbs = 20,
                PercentProtein = 50,
                PercentFat = 30,
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeNutritionGoalAsync(request);

            //Assert
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task ChangeNutritionGoalAsync_ReturnsUnauthorized()
        {
            //Arrange
            var request = new NutritionGoalRequest
            {
                Calories = 2000,
                PercentCarbs = 20,
                PercentProtein = 50,
                PercentFat = 30,
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeNutritionGoalAsync(request);

            //Assert
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeNutritionGoalAsync_ReturnsInvalidParameters_CaloriesAreTooLow()
        {
            //Arrange
            var userId = 1;
            var request = new NutritionGoalRequest
            {
                Calories = 99,
                PercentCarbs = 20,
                PercentProtein = 50,
                PercentFat = 30,
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeNutritionGoalAsync(request);

            //Assert
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
            Assert.IsTrue(result.Errors.Contains("Calories are too low."));
        }

        [Test]
        public async Task ChangeNutritionGoalAsync_ReturnsInvalidParameters_MactrosDontAddUp()
        {
            //Arrange
            var userId = 1;
            var request = new NutritionGoalRequest
            {
                Calories = 2000,
                PercentCarbs = 25,
                PercentProtein = 45,
                PercentFat = 45,
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var goalService = new GoalService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await goalService.ChangeNutritionGoalAsync(request);

            //Assert
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
            Assert.IsTrue(result.Errors.Contains("Macros percentages must add up to 100%."));
        }

        private User GetUserById(Int32 userId)
            => Users.SingleOrDefault(x => x.UserId == userId);
    }
}
