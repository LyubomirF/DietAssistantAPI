using DietAssistant.Business.Contracts.Models.FoodServing.Requests;
using DietAssistant.Business.Contracts.Models.MealFoodLog.Requests;

namespace DietAssistant.Business.Validation
{
    internal static class Validator
    {
        public static Boolean Validate(Int32 id, out String error)
        {
            error = null;

            if(id > 0)  return true;
                
            error = $"Given id {id} is invalid.";
            return false;
        }

        public static Boolean Validate(out List<String> errors, params Int32[] ids)
        {
            errors = new List<String>();

            foreach (var id in ids)
                if (!Validate(id, out String error))
                    errors.Add(error);

            return errors.Count <= 0;
        }

        public static Boolean Validate(LogUpdateFoodServingRequest request, out List<String> errors)
        {
            errors = new List<String>();

            if (request == null)
            {
                errors.Add("Request model cannot be null.");

                return false;
            }

            if (String.IsNullOrEmpty(request.FoodId))
            {
                errors.Add("FoodId is required.");
            }

            if (request.ServingSize <= 0)
            {
                errors.Add("Serving size cannot be less than or equal 0.");
            }

            if (String.IsNullOrEmpty(request.Unit))
            {
                errors.Add("Unit is required.");
            }

            if (request.NumberOfServings <= 0)
            {
                errors.Add("Number of servings cannot be less than or equal 0.");
            }

            return errors.Count <= 0;
        }

        public static Boolean Validate(Int32 mealId, LogUpdateFoodServingRequest request, out List<String> errors)
        {
            errors = new List<String>();

            if (!Validate(mealId, out String err))
            {
                errors.Add(err);
            }

            if (!Validate(request, out List<String> errs))
            {
                errors.AddRange(errs);
            }

            return errors.Count <= 0;
        }

        public static Boolean Validate(Int32 mealId, Int32 foodServingId, LogUpdateFoodServingRequest request, out List<String> errors)
        {
            errors = new List<String>();

            if (!Validate(out List<String> errs1, mealId, foodServingId))
            {
                errors.AddRange(errs1);
            }

            if (!Validate(request, out List<String> errs2))
            {
                errors.AddRange(errs2);
            }

            return errors.Count <= 0;
        }

        public static Boolean Validate(LogMealRequest request, out List<String> errors)
        {
            errors = new List<String>();

            if (request == null)
            {
                errors.Add("Request model cannot be null.");

                return false;
            }

            if(request.FoodServings is null || request.FoodServings.Count <= 0)
            {
                errors.Add("Food servings cannot be null or empty.");

                return false;
            }

            foreach (var foodServings in request.FoodServings)
                if(!Validate(foodServings, out List<String> errorList))
                    errors.AddRange(errorList);

            return errors.Count <= 0;
        } 

        public static Boolean Validate(Int32 id, UpdateMealLogRequest request, out List<String> errors)
        {
            errors = new List<String>();

            if(!Validate(id, out String err))
            {
                errors.Add(err);
            }    

            if (request == null)
            {
                errors.Add("Request model cannot be null.");

                return false;
            }

            foreach (var foodServings in request.FoodServings)
                if (!Validate(foodServings, out List<String> errorList))
                    errors.AddRange(errorList);

            return errors.Count <= 0;
        }
    }
}
