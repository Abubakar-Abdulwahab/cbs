using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotGenerateCashFlowInvoiceException : Exception
    {
        public CouldNotGenerateCashFlowInvoiceException()
        {
        }

        public CouldNotGenerateCashFlowInvoiceException(string message) : base(message)
        {
        }

        public CouldNotGenerateCashFlowInvoiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotGenerateCashFlowInvoiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}