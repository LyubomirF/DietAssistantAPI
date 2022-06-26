using DietAssistant.Domain.DietPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IDietPlanRepository : IRepository<DietPlan>
    {
        public Task<DietPlan> GetDietPlanAsync(Int32 dietPlanId, Int32 userId);
    }
}
