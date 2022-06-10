using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.Business.Contracts.Models.MealFoodLog.Responses
{
    public class LoggedFood
    {
        public Int32 FoodId { get; set; }

        public String FoodName { get; set; }

        public LoggedNutrition Nutrition { get; set; }

    }
}
