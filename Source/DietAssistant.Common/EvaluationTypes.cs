using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DietAssistant.Common
{
    public enum EvaluationTypes
    {
        Success = 1,
        InvalidParameters = 2,
        NotFound = 3,
        Unauthorized = 4,
        Failed = 5,
    }
}
