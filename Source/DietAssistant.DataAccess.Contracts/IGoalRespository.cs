﻿using DietAssistant.Domain;

namespace DietAssistant.DataAccess.Contracts
{
    public interface IGoalRespository : IRepository<Goal>
    {
        Task<Goal> GetGoalByUserIdAsync(Int32 userId);
    }
}
