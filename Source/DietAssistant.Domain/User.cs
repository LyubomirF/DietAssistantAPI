#pragma warning disable CS8618

using DietAssistant.Domain.DietPlanning;

namespace DietAssistant.Domain
{
    public class User
    {
        public Int32 UserId { get; set; }

        public String Email { get; set; }

        public String PasswordHash { get; set; }

        public String Name { get; set; }

        public UserStats UserStats { get; set; }

        public Goal Goal { get; set; }

        public ICollection<ProgressLog> ProgressLogs { get; set; }

        public ICollection<Meal> Meals { get; set; }

        public ICollection<DietPlan> DietPlans { get; set; } 
    }
}
