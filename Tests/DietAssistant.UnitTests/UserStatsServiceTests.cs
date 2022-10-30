using DietAssistant.Business;
using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.UserStats.Requests;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
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

    public class UserStatsServiceTests
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
        public async Task GetUserStatsAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.GetUserStatsAsync();

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Gender == "Male");
            Assert.IsTrue(data.Height == 176);
            Assert.IsTrue(data.HeightUnit == "Centimeters");
            Assert.IsTrue(data.Weight == 85);
            Assert.IsTrue(data.WeightUnit == "Kilograms");
        }

        [Test]
        public async Task GetUserStatsAsync_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.GetUserStatsAsync();

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task GetUserStatsAsync_ReturnsUnauthorized()
        {
            //Arrange
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.GetUserStatsAsync();

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task SetUserStatsAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 2;
            var reqiest = new UserStatsRequest()
            {
                HeightUnit = "Centimeters",
                WeightUnit = "Kilograms",
                Height = 170,
                Weight = 83,
                Gender = "Male",
                DateOfBirth = new DateTime(1995, 1, 10)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.SetUserStatsAsync(reqiest);

            //Arrange
            var user = Users.SingleOrDefault(x => x.UserId == userId);
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsNotNull(user.UserStats);
            Assert.IsNotNull(user.Goal);
            Assert.IsNotEmpty(user.ProgressLogs);
            Assert.IsTrue(data.Height == 170);
            Assert.IsTrue(data.Weight == 83);
        }

        [Test]
        public async Task SetUserStatsAsync_ReturnsFailed_WhenStatsAreAlreadySet()
        {
            //Arrange
            var userId = 1;
            var reqiest = new UserStatsRequest()
            {
                HeightUnit = "Centimeters",
                WeightUnit = "Kilograms",
                Height = 170,
                Weight = 83,
                Gender = "Male",
                DateOfBirth = new DateTime(1995, 1, 10)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.SetUserStatsAsync(reqiest);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Failed);
        }

        [Test]
        public async Task SetUserStatsAsync_ReturnsUnauthorized()
        {
            //Arrange
            var reqiest = new UserStatsRequest()
            {
                HeightUnit = "Centimeters",
                WeightUnit = "Kilograms",
                Height = 170,
                Weight = 83,
                Gender = "Male",
                DateOfBirth = new DateTime(1995, 1, 10)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.SetUserStatsAsync(reqiest);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task SetUserStatsAsync_ReturnsInvalidParameters_InvalidGenderType()
        {
            //Arrange
            var reqiest = new UserStatsRequest()
            {
                HeightUnit = "Centimeters",
                WeightUnit = "Kilograms",
                Height = 170,
                Weight = 83,
                Gender = "Other",
                DateOfBirth = new DateTime(1995, 1, 10)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.SetUserStatsAsync(reqiest);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
            Assert.IsTrue(result.Errors.Contains("Invalid gender type."));
        }

        [Test]
        public async Task SetUserStatsAsync_ReturnsInvalidParameters_InvalidWeightUnit()
        {
            //Arrange
            var reqiest = new UserStatsRequest()
            {
                HeightUnit = "Centimeters",
                WeightUnit = "Stones",
                Height = 170,
                Weight = 83,
                Gender = "Male",
                DateOfBirth = new DateTime(1995, 1, 10)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.SetUserStatsAsync(reqiest);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
            Assert.IsTrue(result.Errors.Contains("Invalid weight unit type."));
        }

        [Test]
        public async Task SetUserStatsAsync_ReturnsInvalidParameters_InvalidHeightUnit()
        {
            //Arrange
            var reqiest = new UserStatsRequest()
            {
                HeightUnit = "FeetInches",
                WeightUnit = "Kilograms",
                Height = 170,
                Weight = 83,
                Gender = "Male",
                DateOfBirth = new DateTime(1995, 1, 10)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.SetUserStatsAsync(reqiest);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
            Assert.IsTrue(result.Errors.Contains("Invalid height unit type."));
        }

        [Test]
        public async Task ChangeHeightUnitAsync_ReturnsSuccess_WhenChangingToInches()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeHeightUnitRequest
            {
                HeightUnit = "Inches"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeHeightUnitAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Height == 69.29);
            Assert.IsTrue(data.HeightUnit == "Inches");
        }

        [Test]
        public async Task ChangeHeightUnitAsync_ReturnsSuccess_WhenChangingToCentimeters()
        {
            //Arrange
            var userId = 3;
            var request = new ChangeHeightUnitRequest
            {
                HeightUnit = "Centimeters"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeHeightUnitAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Height == 177.8);
            Assert.IsTrue(data.HeightUnit == "Centimeters");
        }

        [Test]
        public async Task ChangeHeightUnitAsync_ReturnsSuccess_WhenNoChange()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeHeightUnitRequest
            {
                HeightUnit = "Centimeters"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeHeightUnitAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Height == 176);
        }

        [Test]
        public async Task ChangeHeightUnitAsync_ReturnsUnauthorized()
        {
            //Arrange
            var request = new ChangeHeightUnitRequest
            {
                HeightUnit = "Centimeters"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeHeightUnitAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeHeightUnitAsync_ReturnsInvalidParameters_InvalidHeightUnit()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeHeightUnitRequest
            {
                HeightUnit = "Meters"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeHeightUnitAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
            Assert.IsTrue(result.Errors.Contains("Invalid height unit type."));
        }

        [Test]
        public async Task ChangeWeightUnitAsync_ReturnsSuccess_WhenChangingToPounds()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeWeightUnitRequest
            {
                WeightUnit = "Pounds"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeWeightUnitAsync(request);

            //Arrange
            var user = Users.SingleOrDefault(x => x.UserId == userId);
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Weight == 187.42);
            Assert.IsTrue(data.WeightUnit == "Pounds");
            Assert.IsTrue(user.Goal.StartWeight == 209.48);
            Assert.IsTrue(user.Goal.GoalWeight == 176.4);
        }

        [Test]
        public async Task ChangeWeightUnitAsync_ReturnsSuccess_WhenChangingToKilograms()
        {
            //Arrange
            var userId = 3;
            var request = new ChangeWeightUnitRequest
            {
                WeightUnit = "Kilograms"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeWeightUnitAsync(request);

            //Arrange
            var user = Users.SingleOrDefault(x => x.UserId == userId);
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Weight == 84.81);
            Assert.IsTrue(data.WeightUnit == "Kilograms");
            Assert.IsTrue(user.Goal.StartWeight == 94.78);
            Assert.IsTrue(user.Goal.GoalWeight == 79.82);
        }

        [Test]
        public async Task ChangeWeightUnitAsync_ReturnsSuccess_WhenNoChange()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeWeightUnitRequest
            {
                WeightUnit = "Kilograms"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeWeightUnitAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Weight == 85);
        }

        [Test]
        public async Task ChangeWeightUnitAsync_ReturnsUnauthorized()
        {
            //Arrange
            var request = new ChangeWeightUnitRequest
            {
                WeightUnit = "Kilograms"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeWeightUnitAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeWeightUnitAsync_ReturnsInvalidParameters_InvalidWeightUnit()
        {
            //Arrange
            var request = new ChangeWeightUnitRequest
            {
                WeightUnit = "Stones"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeWeightUnitAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
            Assert.IsTrue(result.Errors.Contains("Invalid weight unit type."));
        }

        [Test]
        public async Task ChangeWeightAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;

            var request = new ChangeWeightRequest
            {
                Weight = 86
            };
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            var user = Users.SingleOrDefault(x => x.UserId == userId);
            var progressLogsCount = user.ProgressLogs.Count;

            //Act
            var result = await userStatsService.ChangeWeightAsync(request);

            //Arrange
            var updatedUser = Users.SingleOrDefault(x => x.UserId == userId);
            var logsCount = updatedUser.ProgressLogs.Count;
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Weight == 86);
            Assert.IsTrue(user.Goal.CurrentWeight == 86);
            Assert.IsTrue(progressLogsCount + 1 == logsCount);
        }

        [Test]
        public async Task ChangeWeightAsync_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;

            var request = new ChangeWeightRequest
            {
                Weight = 86
            };
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeWeightAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
            Assert.IsTrue(result.Errors.Contains("User stats are not set."));
        }

        [Test]
        public async Task ChangeWeightAsync_ReturnsUnauthorized()
        {
            //Arrange
            var request = new ChangeWeightRequest
            {
                Weight = 86
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeWeightAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeHeightAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;

            var request = new ChangeHeightRequest
            {
                Height = 176.2
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeHeightAsync(request);

            //Assert
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Height == 176.2);
        }

        [Test]
        public async Task ChangeHeightAsync_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;

            var request = new ChangeHeightRequest
            {
                Height = 176.2
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeHeightAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task ChangeHeightAsync_ReturnsUnauthorized()
        {
            //Arrange
            var request = new ChangeHeightRequest
            {
                Height = 176.2
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeHeightAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeGenderAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeGenderRequest
            {
                Gender = "Female"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeGenderAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Gender == "Female");
        }

        [Test]
        public async Task ChangeGenderAsync_ReturnsSuccess_WhenNoChange()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeGenderRequest
            {
                Gender = "Male"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeGenderAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Gender == "Male");
        }

        [Test]
        public async Task ChangeGenderAsync_ReturnsInvalidParameters_InvalidGender()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeGenderRequest
            {
                Gender = "Other"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeGenderAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
        }

        [Test]
        public async Task ChangeGenderAsync_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;
            var request = new ChangeGenderRequest
            {
                Gender = "Female"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeGenderAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task ChangeGenderAsync_ReturnsUnauthorized()
        {
            //Arrange
            var request = new ChangeGenderRequest
            {
                Gender = "Female"
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeGenderAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task ChangeDateOfBirth_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeDateOfBirthRequest
            {
                DateOfBirth = new DateTime(1999, 5, 15)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeDateOfBirthAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.DateOfBirth == "15 May 1999");
        }

        [Test]
        public async Task ChangeDateOfBirth_ReturnsNotFound()
        {
            //Arrange
            var userId = 2;
            var request = new ChangeDateOfBirthRequest
            {
                DateOfBirth = new DateTime(1999, 5, 15)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeDateOfBirthAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }

        [Test]
        public async Task ChangeDateOfBirth_ReturnsUnauthorized()
        {
            //Arrange
            var userId = 1;
            var request = new ChangeDateOfBirthRequest
            {
                DateOfBirth = new DateTime(1999, 5, 15)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var userStatsService = new UserStatsService(
                _userResolverServiceMock.Object,
                _userRepository);

            //Act
            var result = await userStatsService.ChangeDateOfBirthAsync(request);

            //Arrange
            var data = result.Data;

            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }
    }
}
