using DietAssistant.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IMealRepository : IRepository<Meal>
    {
        Task<Meal> GetMealByIdWithFoodServings(Int32 id);

        Task<IEnumerable<Meal>> GetMealsForDayAsync(DateTime date);

        Task<Meal> GetLastMealAsync(DateTime date);
    }
}
