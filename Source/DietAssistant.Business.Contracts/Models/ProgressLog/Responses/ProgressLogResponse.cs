using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.Business.Contracts.Models.ProgressLog.Responses
{
    public class ProgressLogResponse
    {
        public String MeasurementType { get; set; }

        public Double Measurement { get; set; }

        public DateTime LoggedOn { get; set; }
    }
}
