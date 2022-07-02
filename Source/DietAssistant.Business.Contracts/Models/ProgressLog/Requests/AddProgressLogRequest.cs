namespace DietAssistant.Business.Contracts.Models.ProgressLog.Requests
{
    public class AddProgressLogRequest
    {
        public String MeasurementType { get; set; }

        public Double Measurement { get; set; }

        public DateTime Date { get; set; }
    }
}
