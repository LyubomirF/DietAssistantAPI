using AutoFixture;
using DietAssistant.Business;
using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.ProgressLog.Requests;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.Domain.Enums;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DietAssistant.UnitTests
{
    public class Tests : TestsBase
    {
        private Mock<IUserResolverService> _userResolverServiceMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IProgressLogRepository> _progressLogRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _userResolverServiceMock = new Mock<IUserResolverService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _progressLogRepositoryMock = new Mock<IProgressLogRepository>();
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 123;
            var count = 5;

            var progressLogs = _fixture.Build<ProgressLog>()
                .With(x => x.UserId, userId)
                .With(x => x.MeasurementType, MeasurementType.Weight)
                .Without(x => x.User)
                .CreateMany(count);

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            _progressLogRepositoryMock.Setup(x => x.GetProgressLogPagedAsync(
                It.IsAny<Int32>(),
                It.IsAny<MeasurementType>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<Int32>(),
                It.IsAny<Int32>()))
                .ReturnsAsync((progressLogs, count));

            var progressService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepositoryMock.Object,
                _progressLogRepositoryMock.Object);

            var req = _fixture.Build<ProgressLogFilterRequest>()
                .With(x => x.MeasurementType, "Weight")
                .Create();

            //Act
            var result = await progressService.GetProgressLogsPagedAsync(req);

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(result.Data.TotalCount == count);
            Assert.IsTrue(result.Data.Page == req.Page);
            Assert.IsTrue(result.Data.PageSize == req.PageSize);
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsUnauthorized()
        {
            //Arrange
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var progressService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepositoryMock.Object,
                _progressLogRepositoryMock.Object);

            var req = _fixture.Create<ProgressLogFilterRequest>();

            //Act
            var result = await progressService.GetProgressLogsPagedAsync(req);

            //Assert
            Assert.IsFalse(result.IsSuccessful());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }


        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsInvalidParameters()
        {
            //Arrange
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => 123);

            var progressService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepositoryMock.Object,
                _progressLogRepositoryMock.Object);

            var req = _fixture.Build<ProgressLogFilterRequest>()
                .With(x => x.MeasurementType, "Steps")
                .Create();

            //Act
            var result = await progressService.GetProgressLogsPagedAsync(req);

            //Assert
            Assert.IsFalse(result.IsSuccessful());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
        }

        [Test]
        public async Task AddProgressLogAsync_ReturnsSuccess()
        {
            var userId = 123;

            //Arrange
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => userId);

            var progressLogs = _fixture.Build<ProgressLog>()
                .With(x => x.UserId, userId)
                .Without(x => x.User)
                .CreateMany(10)
                .ToList();

            var userStats = _fixture.Build<UserStats>()
                .Without(x => x.User)
                .Create();

            var user = _fixture.Build<User>()
                .With(x => x.UserId, userId)
                .With(x => x.ProgressLogs, progressLogs)
                .With(x => x.UserStats, userStats)
                .Without(x => x.Goal)
                .Without(x => x.NutritionGoals)
                .Without(x => x.Meals)
                .Without(x => x.DietPlans)
                .Create();

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Int32>()))
                .ReturnsAsync(user);

            var progressService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepositoryMock.Object,
                _progressLogRepositoryMock.Object);

            var req = _fixture.Build<AddProgressLogRequest>()
                .With(x => x.MeasurementType, MeasurementType.Neck.ToString())
                .Create();

            //Act
            var result = await progressService.AddProgressLogAsync(req);

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(result.Data.MeasurementType == MeasurementType.Neck.ToString());

            _progressLogRepositoryMock.Verify(x => x.SaveEntityAsync(It.IsAny<ProgressLog>()), Times.Once);
        }

        [Test]
        public async Task AddProgressLogAsync_ReturnsSuccessWhenWeightIsLogged()
        {
            var userId = 123;

            //Arrange
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => userId);

            var progressLogs = _fixture.Build<ProgressLog>()
                .With(x => x.UserId, userId)
                .Without(x => x.User)
                .CreateMany(10)
                .ToList();

            var userStats = _fixture.Build<UserStats>()
                .Without(x => x.User)
                .Create();

            var goal = _fixture.Build<Goal>()
                .Without(x => x.User)
                .Without(x => x.NutritionGoal)
                .Create();

            var user = _fixture.Build<User>()
                .With(x => x.UserId, userId)
                .With(x => x.ProgressLogs, progressLogs)
                .With(x => x.UserStats, userStats)
                .With(x => x.Goal, goal)
                .Without(x => x.NutritionGoals)
                .Without(x => x.Meals)
                .Without(x => x.DietPlans)
                .Create();

            _userRepositoryMock.Setup(x => x.GetUserByIdAsync(It.IsAny<Int32>()))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(x =>
                x.UpdateCurrentWeightAsync(It.IsAny<User>(), It.IsAny<Double>(), It.IsAny<WeeklyGoal>(), It.IsAny<Double>()))
                .ReturnsAsync(user);

            var progressService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepositoryMock.Object,
                _progressLogRepositoryMock.Object);

            var req = _fixture.Build<AddProgressLogRequest>()
                .With(x => x.MeasurementType, MeasurementType.Weight.ToString())
                .Create();

            //Act
            var result = await progressService.AddProgressLogAsync(req);

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.AreEqual(user.ProgressLogs.LastOrDefault().MeasurementType.ToString(), result.Data.MeasurementType);

            _userRepositoryMock.Verify(x => x.UpdateCurrentWeightAsync(
                It.IsAny<User>(),
                It.IsAny<Double>(),
                It.IsAny<WeeklyGoal>(),
                It.IsAny<Double>()),
                Times.Once);
        }

        [Test]
        public async Task DeleteProgressLogAsync_ReturnsSuccess()
        {
            var userId = 123;

            //Arrange
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => userId);

            var progressLog = _fixture.Build<ProgressLog>()
                .With(x => x.UserId, userId)
                .Without(x => x.User)
                .Create();

            _progressLogRepositoryMock.Setup(x => x.GetProgressLogAsync(userId, It.IsAny<Int32>()))
                .ReturnsAsync(progressLog);

            _progressLogRepositoryMock.Setup(x => x.DeleteProgressLog(progressLog))
                .ReturnsAsync(1);

            var progressService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepositoryMock.Object,
                _progressLogRepositoryMock.Object);

            //Act
            var result = await progressService.DeleteProgressLogAsync(It.IsAny<Int32>());

            //Assert
            Assert.IsTrue(result.IsSuccessful());
        }

        [Test]
        public async Task DeleteProgressLogAsync_ReturnsFailed()
        {
            var userId = 123;

            //Arrange
            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => userId);

            var progressLog = _fixture.Build<ProgressLog>()
                .With(x => x.UserId, userId)
                .Without(x => x.User)
                .Create();

            _progressLogRepositoryMock.Setup(x => x.GetProgressLogAsync(userId, It.IsAny<Int32>()))
                .ReturnsAsync(progressLog);

            _progressLogRepositoryMock.Setup(x => x.DeleteProgressLog(progressLog))
                .ReturnsAsync(0);

            var progressService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepositoryMock.Object,
                _progressLogRepositoryMock.Object);

            //Act
            var result = await progressService.DeleteProgressLogAsync(It.IsAny<Int32>());

            //Assert
            Assert.IsTrue(result.IsFailure());
        }
    }
}