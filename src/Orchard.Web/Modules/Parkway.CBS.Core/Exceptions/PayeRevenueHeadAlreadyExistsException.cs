using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class PayeRevenueHeadAlreadyExistsException : Exception
    {
        public PayeRevenueHeadAlreadyExistsException()
        {
        }

        public PayeRevenueHeadAlreadyExistsException(string message) : base(message)
        {
        }

        public PayeRevenueHeadAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PayeRevenueHeadAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}