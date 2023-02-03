using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotCreateReceiptException : Exception
    { 
        public CannotCreateReceiptException()
        {
        }

        public CannotCreateReceiptException(string message) : base(message)
        {
        }

        public CannotCreateReceiptException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotCreateReceiptException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}