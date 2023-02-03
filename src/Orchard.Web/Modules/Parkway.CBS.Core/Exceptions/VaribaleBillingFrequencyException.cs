using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class BillingFrequencyException : Exception
    {
        public BillingFrequencyException()
        {
        }

        public BillingFrequencyException(string message) : base(message)
        {
        }

        public BillingFrequencyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BillingFrequencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}