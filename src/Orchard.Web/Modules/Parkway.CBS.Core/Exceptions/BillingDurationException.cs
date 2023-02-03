using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class BillingDurationException : Exception
    {
        public BillingDurationException()
        {
        }

        public BillingDurationException(string message) : base(message)
        {
        }

        public BillingDurationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BillingDurationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}