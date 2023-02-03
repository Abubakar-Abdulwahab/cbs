using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class AlreadyHasBillingException : Exception
    {
        public AlreadyHasBillingException()
        {
        }

        public AlreadyHasBillingException(string message) : base(message)
        {
        }

        public AlreadyHasBillingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyHasBillingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
