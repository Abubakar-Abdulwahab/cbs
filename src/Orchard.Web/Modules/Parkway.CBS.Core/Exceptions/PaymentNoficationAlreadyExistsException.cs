using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class PaymentNoficationAlreadyExistsException : Exception
    {
        public PaymentNoficationAlreadyExistsException()
        {
        }

        public PaymentNoficationAlreadyExistsException(string message) : base(message)
        {
        }

        public PaymentNoficationAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PaymentNoficationAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}