using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class BillingHasEndedException : Exception
    {
        public BillingHasEndedException()
        {
        }

        public BillingHasEndedException(string message) : base(message)
        {
        }

        public BillingHasEndedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BillingHasEndedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}