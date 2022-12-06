using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using DietAssistant.UnitTests.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable

namespace DietAssistant.UnitTests.Repositories
{
    using static DatabaseMock;

    internal class MealRepositoryMock : IRepository<Meal>, IMealRepository
    {
        public async Task<IEnumerable<Meal>> GetMealsForDayAsync(DateTime date, Int32 userId)
        {
            var user = Users.SingleOrDefault(x => x.UserId == userId);

            var meals = user?.Meals
                .Where(x => x.EatenOn.Date == date.Date)
                .OrderBy(x => x.Order);

            if(meals == null)
            {
                return await Task.FromResult(Enumerable.Empty<Meal>());
            }

            return await Task.FromResult(meals);
        }

        public Task<Meal> GetLastMealAsync(DateTime date, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<Meal> GetMealByIdWithFoodServingsAsync(Int32 mealId, Int32 userId)
        {
            var user = Users.SingleOrDefault(x => x.UserId == userId);

            var meal = user.Meals.SingleOrDefault(x => x.MealId == mealId);

            return Task.FromResult(meal);
        }

        public Task<Meal> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteFoodServingAsync(Meal meal, FoodServing foodServing)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteMealAsync(Meal meal)
        {
            throw new NotImplementedException();
        }

        public Task SaveEntityAsync(Meal entity)
        {
            throw new NotImplementedException();
        }
    }
}
