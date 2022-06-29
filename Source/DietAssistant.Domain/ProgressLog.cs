#pragma warning disable CS8618

namespace DietAssistant.Domain
{
    public class ProgressLog
    {
        public Int32 ProgressLogId { get; set; }

        public User User { get; set; }

        public Int32 UserId { get; set; }

        public Double Weigth { get; set; }

        public DateTime LoggedOn { get; set; }
    }
}
