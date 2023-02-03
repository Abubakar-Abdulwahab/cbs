using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class InvoiceAlreadyPaidForException : Exception
    {
        public InvoiceAlreadyPaidForException()
        {
        }

        public InvoiceAlreadyPaidForException(string message) : base(message)
        {
        }

        public InvoiceAlreadyPaidForException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvoiceAlreadyPaidForException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}