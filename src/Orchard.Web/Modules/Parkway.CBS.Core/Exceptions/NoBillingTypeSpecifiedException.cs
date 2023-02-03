using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NoBillingTypeSpecifiedException : Exception
    {
        public NoBillingTypeSpecifiedException()
        {
        }

        public NoBillingTypeSpecifiedException(string message) : base(message)
        {
        }

        public NoBillingTypeSpecifiedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoBillingTypeSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}