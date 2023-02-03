using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException : Exception
    {
        public CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException()
        {
        }

        public CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException(string message) : base(message)
        {
        }

        public CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
