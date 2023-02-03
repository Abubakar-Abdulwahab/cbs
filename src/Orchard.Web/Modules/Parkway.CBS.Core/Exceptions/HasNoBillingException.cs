using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class HasNoBillingException : Exception
    {
        public HasNoBillingException()
        {
        }

        public HasNoBillingException(string message) : base(message)
        {
        }

        public HasNoBillingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HasNoBillingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
