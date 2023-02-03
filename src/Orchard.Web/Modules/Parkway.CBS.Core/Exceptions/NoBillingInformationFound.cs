using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NoBillingInformationFoundException : Exception
    {
        public NoBillingInformationFoundException()
        {
        }

        public NoBillingInformationFoundException(string message) : base(message)
        {
        }

        public NoBillingInformationFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoBillingInformationFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}