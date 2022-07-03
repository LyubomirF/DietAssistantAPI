using DietAssistant.Business.Contracts;
using DietAssistant.Business.Contracts.Models.Goal.Responses;
using DietAssistant.Business.Mappers;
using DietAssistant.Common;
using DietAssistant.DataAccess.Contracts;

namespace DietAssistant.Business
{
    public class GoalService : IGoalService
    {
        private readonly IUserResolverService _userResolverService;
        private readonly IGoalRespository _goalRespository;

        public GoalService(IUserResolverService userResolverService, IGoalRespository goalRespository)
        {
            _userResolverService = userResolverService;
            _goalRespository = goalRespository;
        }

        public async Task<Result<GoalResponse>> GetGoalAsync()
        {
            var currentUserId = _userResolverService.GetCurrentUserId();

            if (!currentUserId.HasValue)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.Unauthorized, ResponseMessages.Unauthorized);

            var goal = await _goalRespository.GetGoalByUserIdAsync(currentUserId.Value);

            if(goal is null)
                return Result
                    .CreateWithError<GoalResponse>(EvaluationTypes.NotFound, "Goal was not found.");

            return Result.Create(goal.ToResponse());
        }
    }
}
