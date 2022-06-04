using DietAssistant.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IFoodServingRepository : IRepository<FoodServing>
    {
    }
}
