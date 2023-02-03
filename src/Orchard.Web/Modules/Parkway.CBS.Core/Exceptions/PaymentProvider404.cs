using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class PaymentProvider404 : Exception
    {
        public PaymentProvider404()
        {
        }
        public PaymentProvider404(string message) : base(message)
        {
        }
        public PaymentProvider404(string message, Exception innerException) : base(message, innerException)
        {
        }
        protected PaymentProvider404(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}