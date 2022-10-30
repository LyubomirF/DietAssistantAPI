using DietAssistant.Business;
using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.ProgressLog.Requests;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain.Enums;
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

    public class ProgressLogServiceTests
    {
        private IProgressLogRepository _progressLogRepository;
        private IUserRepository _userRepository;

        private Mock<IUserResolverService> _userResolverServiceMock;

        [SetUp]
        public void Setup()
        {

            DatabaseMock.Initialize();
            _progressLogRepository = new ProgressLogRepositoryMock();
            _userRepository = new UserRepositoryMock();
            _userResolverServiceMock = new Mock<IUserResolverService>();
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsWeightLogsWithPageSize()
        {
            //Arrange
            var page = 1;
            var pageSize = 2;

            var request = new ProgressLogFilterRequest
            {
                Page = page,
                PageSize = pageSize,
                MeasurementType = "Weight",
                PeriodStart = null,
                PeriodEnd = null
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.GetProgressLogsPagedAsync(request);

            var data = result.Data;

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Results.TrueForAll(x => x.MeasurementType == "Weight"));
            Assert.IsFalse(data.TotalCount == data.Results.Count);
            Assert.IsTrue(data.Page == 1);
            Assert.IsTrue(data.PageSize == 2);
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsWeightLogsWithPage()
        {
            //Arrange
            var page = 2;
            var pageSize = 2;

            var request = new ProgressLogFilterRequest
            {
                Page = page,
                PageSize = pageSize,
                MeasurementType = "Weight",
                PeriodStart = null,
                PeriodEnd = null
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.GetProgressLogsPagedAsync(request);

            var data = result.Data;

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Results.TrueForAll(x => x.MeasurementType == "Weight"));
            Assert.IsFalse(data.TotalCount == data.Results.Count);
            Assert.IsTrue(data.Page == 2);
            Assert.IsTrue(data.PageSize == 2);
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsWeightLogs()
        {
            //Arrange
            var request = new ProgressLogFilterRequest
            {
                MeasurementType = "Weight",
                PeriodStart = null,
                PeriodEnd = null
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.GetProgressLogsPagedAsync(request);

            var data = result.Data;

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Results.TrueForAll(x => x.MeasurementType == "Weight"));
            Assert.IsTrue(data.TotalCount == data.Results.Count);
            Assert.IsTrue(data.Page == 1);
            Assert.IsTrue(data.PageSize == 20);
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsWaistLogs()
        {
            //Arrange
            var request = new ProgressLogFilterRequest
            {
                MeasurementType = "Waist",
                PeriodStart = null,
                PeriodEnd = null
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.GetProgressLogsPagedAsync(request);

            var data = result.Data;

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Results.TrueForAll(x => x.MeasurementType == "Waist"));
            Assert.IsTrue(data.TotalCount == data.Results.Count);
            Assert.IsTrue(data.Page == 1);
            Assert.IsTrue(data.PageSize == 20);
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsWeightLogsInPeriod()
        {
            //Arrange
            DateTime? periodStart = new DateTime(2021, 10, 2);
            DateTime? periodEnd = new DateTime(2021, 10, 6);

            var request = new ProgressLogFilterRequest
            {
                MeasurementType = "Weight",
                PeriodStart = periodStart,
                PeriodEnd = periodEnd
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.GetProgressLogsPagedAsync(request);

            var data = result.Data;

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Results.TrueForAll(x => x.MeasurementType == "Weight"));
            Assert.IsTrue(data.TotalCount == 5);
            Assert.IsTrue(data.Page == 1);
            Assert.IsTrue(data.PageSize == 20);
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsWeightLogsAfterDate()
        {
            //Arrange
            DateTime? periodStart = new DateTime(2021, 10, 2);
            DateTime? periodEnd = null;

            var request = new ProgressLogFilterRequest
            {
                MeasurementType = "Weight",
                PeriodStart = periodStart,
                PeriodEnd = periodEnd
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.GetProgressLogsPagedAsync(request);

            var data = result.Data;

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Results.TrueForAll(x => x.MeasurementType == "Weight"));
            Assert.IsTrue(data.TotalCount == 6);
            Assert.IsTrue(data.Page == 1);
            Assert.IsTrue(data.PageSize == 20);
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsWeightLogsBeforeDate()
        {
            //Arrange
            DateTime? periodStart = null;
            DateTime? periodEnd = new DateTime(2021, 10, 3);

            var request = new ProgressLogFilterRequest
            {
                MeasurementType = "Weight",
                PeriodStart = periodStart,
                PeriodEnd = periodEnd
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.GetProgressLogsPagedAsync(request);

            var data = result.Data;

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.Results.TrueForAll(x => x.MeasurementType == "Weight"));
            Assert.IsTrue(data.Results.TrueForAll(x => x.LoggedOn <= periodEnd));
            Assert.IsTrue(data.Page == 1);
            Assert.IsTrue(data.PageSize == 20);
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsUnauthorizedResponse()
        {
            //Arrange
            var request = new ProgressLogFilterRequest
            {
                MeasurementType = "Weight",
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.GetProgressLogsPagedAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task GetProgressLogsPagedAsync_ReturnsInvalidParameterResponse()
        {
            //Arrange
            var request = new ProgressLogFilterRequest
            {
                MeasurementType = "Steps",
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.GetProgressLogsPagedAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.InvalidParameters);
        }

        [Test]
        public async Task AddProgressLogAsync_ReturnsSuccess_WhenAddingWaistMeasurement()
        {
            //Arrange
            var request = new AddProgressLogRequest
            {
                MeasurementType = "Waist",
                Measurement = 85,
                Date = new DateTime(2022, 1, 1)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            var progressCount = Users.SingleOrDefault(x => x.UserId == 1).ProgressLogs.Count;

            //Act
            var result = await progressLogService.AddProgressLogAsync(request);

            //Arrange
            var progressCountAfterAdd = Users.SingleOrDefault(x => x.UserId == 1).ProgressLogs.Count;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(progressCount + 1 == progressCountAfterAdd);
            Assert.IsTrue(result.Data.MeasurementType == "Waist");
            Assert.IsTrue(result.Data.Measurement == 85);
        }

        [Test]
        public async Task AddProgressLogAsync_ReturnsSuccess_WhenAddingWeightMeasurement()
        {
            //Arrange
            var request = new AddProgressLogRequest
            {
                MeasurementType = "Weight",
                Measurement = 91,
                Date = new DateTime(2022, 1, 1)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(1);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            var user = Users.SingleOrDefault(x => x.UserId == 1);
            var progressCount = user.ProgressLogs.Count;
            var goal = user.Goal.WeeklyGoal;

            //Act
            var result = await progressLogService.AddProgressLogAsync(request);

            //Arrange
            var updatedUser = Users.SingleOrDefault(x => x.UserId == 1);
            var progressCountAfterAdd = updatedUser.ProgressLogs.Count;
            var weeklyGoalAterAdd = updatedUser.Goal.WeeklyGoal;
            var weightUserStats = updatedUser.UserStats.Weight;
            var weightGoal = updatedUser.Goal.CurrentWeight;

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(progressCount + 1 == progressCountAfterAdd);
            Assert.IsTrue(weightUserStats == 91 && weightGoal == 91);
            Assert.IsTrue(weeklyGoalAterAdd == WeeklyGoal.SlowWeightLoss);
            Assert.IsTrue(result.Data.MeasurementType == "Weight");
            Assert.IsTrue(result.Data.Measurement == 91);   
        }

        [Test]
        public async Task AddProgressLogAsync_ReturnsFailedResponse_WhenUserStatsNotSet()
        {
            //Arrange
            var userId = 2;

            var request = new AddProgressLogRequest
            {
                MeasurementType = "Weight",
                Measurement = 91,
                Date = new DateTime(2022, 1, 1)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.AddProgressLogAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Failed);
        }

        [Test]
        public async Task AddProgressLogAsync_ReturnsUnauthorizedResponse()
        {
            //Arrange
            var request = new AddProgressLogRequest
            {
                MeasurementType = "Weight",
                Measurement = 91,
                Date = new DateTime(2022, 1, 1)
            };

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.AddProgressLogAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task DeleteProgressLogAsync_ReturnsSuccess()
        {
            //Arrange 
            var userId = 1;
            var request = 1;

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.DeleteProgressLogAsync(request);

            //Arrange
            var exists = Users
                .SingleOrDefault(x => x.UserId == userId)
                .ProgressLogs
                .Any(x => x.ProgressLogId == request);

            Assert.IsTrue(result.IsSuccessful());
            Assert.IsFalse(exists);
        }

        [Test]
        public async Task DeleteProgressLogAsync_ReturnsUnauthorized()
        {
            //Arrange 
            var userId = 1;
            var request = 1;

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var progressLogService = new ProgressLogService(
                _userResolverServiceMock.Object,
                _userRepository,
                _progressLogRepository);

            //Act
            var result = await progressLogService.DeleteProgressLogAsync(request);

            //Arrange
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }
    }
}
