﻿#pragma warning disable CS8618

namespace DietAssistant.Domain
{
    public class User
    {
        public Int32 UserId { get; set; }

        public String Name { get; set; }

        public UserStats UserStats { get; set; }

        public ICollection<ProgressLog> ProgressLogs { get; set; }

        public ICollection<Meal> Meals { get; set; }
    }
}
