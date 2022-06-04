using DietAssistant.Business.Contracts.Models.Requests;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.Business
{
    public class FoodLogService
    {
        private readonly IRepository<Meal> _mealRepository;
        private readonly IRepository<FoodServing> _foodServingRepository;

        public FoodLogService(
            IRepository<Meal> mealRepository,
            IRepository<FoodServing> foodServingRepository)
        {
            _mealRepository = mealRepository;
            _foodServingRepository = foodServingRepository;
        }

        public async Task LogFood(LogRequest request)
        {
            //TODO: Implement catalog, to find or create the food item
            var food = new Food(); // await _catalogService.FindAsync(id);
            var meal = await _mealRepository.GetByIdAsync(request.MealId);

            var foodServing = new FoodServing
            {
                Food = food,
                Meal = meal,
                ServingSizeAmount = request.ServingSizeAmount,
                ServingSizeUnit = request.ServingSizeUnit,
                NumberOfServings = request.NumberOfServings
            };

            await _foodServingRepository.SaveAsync(foodServing);
        }
    }

}
