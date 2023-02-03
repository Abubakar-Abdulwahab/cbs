using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    public class InvoiceHasPaymentException : Exception
    {
        public InvoiceHasPaymentException()
        {
        }

        public InvoiceHasPaymentException(string message) : base(message)
        {
        }

        public InvoiceHasPaymentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvoiceHasPaymentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


    public class InvoiceCancelledException : Exception
    {
        public InvoiceCancelledException()
        {
        }

        public InvoiceCancelledException(string message) : base(message)
        {
        }

        public InvoiceCancelledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvoiceCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}