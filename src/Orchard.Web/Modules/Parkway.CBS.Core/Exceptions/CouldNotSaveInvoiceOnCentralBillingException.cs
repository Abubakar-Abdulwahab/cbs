using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    public class CouldNotSaveInvoiceOnCentralBillingException : Exception
    {
        public CouldNotSaveInvoiceOnCentralBillingException()
        {
        }

        public CouldNotSaveInvoiceOnCentralBillingException(string message) : base(message)
        {
        }

        public CouldNotSaveInvoiceOnCentralBillingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveInvoiceOnCentralBillingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
