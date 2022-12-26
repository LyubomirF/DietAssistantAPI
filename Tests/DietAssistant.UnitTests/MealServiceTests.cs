using DietAssistant.Business;
using DietAssistant.Business.Contracts;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.UnitTests.Database;
using DietAssistant.UnitTests.Repositories;
using DietAssistant.UnitTests.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable

namespace DietAssistant.UnitTests
{
    using static DatabaseMock;

    internal class MealServiceTests
    {
        private Mock<IUserResolverService> _userResolverServiceMock;
        private IUserRepository _userRepository;
        private IFoodCatalogService _foodCatalogService;
        private IMealRepository _mealRepository;

        [SetUp]
        public void Setup()
        {

            DatabaseMock.Initialize();
            _userRepository = new UserRepositoryMock();
            _foodCatalogService = new FoodCatalogMock();
            _mealRepository = new MealRepositoryMock();
            
            _userResolverServiceMock = new Mock<IUserResolverService>();
        }

        [Test]
        public async Task GetMealsOnDateAsync_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;
            DateTime? request = new DateTime(2021, 12, 1);

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var mealService = new MealService(
                _userResolverServiceMock.Object,
                _userRepository,
                _foodCatalogService,
                _mealRepository);

            //Act
            var result = await mealService.GetMealsOnDateAsync(request);

            var count = 2;
            var meal1 = result.Data.FirstOrDefault();
            var meal2 = result.Data.LastOrDefault();

            //Assert
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(result.Data.Count() == count);
            Assert.IsTrue(meal1.TotalCalories == 260);
            Assert.IsTrue(meal1.TotalCarbs == 12.24);
            Assert.IsTrue(meal1.TotalFat == 4.28);
            Assert.IsTrue(meal1.TotalProtein == 39.2);
            Assert.IsTrue(meal2.TotalCalories == 56.25);
            Assert.IsTrue(meal2.TotalCarbs == 10.5);
            Assert.IsTrue(meal2.TotalFat == 1.5);
            Assert.IsTrue(meal2.TotalProtein == 0.75);
        }

        [Test]
        public async Task GetMealsOnDateAsync_ReturnsUnauthorized()
        {
            //Arrange
            DateTime? request = new DateTime(2021, 12, 1);

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var mealService = new MealService(
                _userResolverServiceMock.Object,
                _userRepository,
                _foodCatalogService,
                _mealRepository);

            //Act
            var result = await mealService.GetMealsOnDateAsync(request);

            //Assert
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task GetMealById_ReturnsSuccess()
        {
            //Arrange
            var userId = 1;
            var mealId = 1;

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var mealService = new MealService(
                _userResolverServiceMock.Object,
                _userRepository,
                _foodCatalogService,
                _mealRepository);

            //Act
            var result = await mealService.GetMealById(mealId);

            //Assert
            var data = result.Data;
            Assert.IsTrue(result.IsSuccessful());
            Assert.IsTrue(data.TotalCalories == 260);
            Assert.IsTrue(data.TotalCarbs == 12.24);
            Assert.IsTrue(data.TotalFat == 4.28);
            Assert.IsTrue(data.TotalProtein == 39.2);
        }

        [Test]
        public async Task GetMealById_ReturnsUnauthorized()
        {
            //Arrange
            var userId = 1;
            var mealId = 1;

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(() => null);

            var mealService = new MealService(
                _userResolverServiceMock.Object,
                _userRepository,
                _foodCatalogService,
                _mealRepository);

            //Act
            var result = await mealService.GetMealById(mealId);

            //Assert
            var data = result.Data;
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.Unauthorized);
        }

        [Test]
        public async Task GetMealById_ReturnsNotFound()
        {
            //Arrange
            var userId = 1;
            var mealId = 111;

            _userResolverServiceMock.Setup(x => x.GetCurrentUserId())
                .Returns(userId);

            var mealService = new MealService(
                _userResolverServiceMock.Object,
                _userRepository,
                _foodCatalogService,
                _mealRepository);

            //Act
            var result = await mealService.GetMealById(mealId);

            //Assert
            var data = result.Data;
            Assert.IsTrue(result.IsFailure());
            Assert.IsTrue(result.EvaluationResult == EvaluationTypes.NotFound);
        }
    }
}
