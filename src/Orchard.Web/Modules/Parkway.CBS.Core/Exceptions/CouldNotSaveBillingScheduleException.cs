using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    internal class CouldNotSaveBillingScheduleException : Exception
    {
        public CouldNotSaveBillingScheduleException()
        {
        }

        public CouldNotSaveBillingScheduleException(string message) : base(message)
        {
        }

        public CouldNotSaveBillingScheduleException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveBillingScheduleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}