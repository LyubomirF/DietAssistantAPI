#pragma warning disable CS8618

using DietAssistant.Domain.Enums;

namespace DietAssistant.Domain
{
    public class ProgressLog
    {
        public Int32 ProgressLogId { get; set; }

        public User User { get; set; }

        public Int32 UserId { get; set; }

        public MeasurementType MeasurementType { get; set; }

        public Double Measurement { get; set; }

        public DateTime LoggedOn { get; set; }
    }
}
