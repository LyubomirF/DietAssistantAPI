using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.Domain
{
    public class UserStats
    {
        public Int32 UserStatsId { get; set; }

        public User User { get; set; }

        public Int32 UserId { get; set; }

        public Double Height { get; set; }

        public Int32 HeightMetricId { get; set; }

        public Metric HeightMetric { get; set; }

        public Double Weight { get; set; }

        public Int32 WeightMetricId { get; set; }

        public Metric WeigthMetric { get; set; }

        public Double BodyFatPercentage { get; set; }
    }
}
