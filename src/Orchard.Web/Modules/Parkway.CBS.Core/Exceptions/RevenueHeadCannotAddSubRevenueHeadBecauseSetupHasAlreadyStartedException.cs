using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException : Exception
    {
        public RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException()
        {
        }

        public RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException(string message) : base(message)
        {
        }

        public RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RevenueHeadCannotAddSubRevenueHeadBecauseSetupHasAlreadyStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
