using DietAssistant.Business.Contracts.Models.Paging;

namespace DietAssistant.Business.Contracts.Models.ProgressLog.Requests
{
    public class ProgressLogFilterRequest : PagingParameters
    {
        public String MeasurementType { get; set; } = "Weight";

        public DateTime? PeriodStart { get; set; }

        public DateTime? PeriodEnd { get; set; }
    }
}
