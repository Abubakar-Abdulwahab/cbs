using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class BillingIsNotAllowedException : Exception
    {
        public BillingIsNotAllowedException()
        {
        }

        public BillingIsNotAllowedException(string message) : base(message)
        {
        }

        public BillingIsNotAllowedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BillingIsNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}