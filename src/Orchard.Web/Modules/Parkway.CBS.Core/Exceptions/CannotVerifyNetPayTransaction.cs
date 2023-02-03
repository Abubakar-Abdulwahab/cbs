using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotVerifyNetPayTransaction : Exception
    {
        public CannotVerifyNetPayTransaction()
        {
        }

        public CannotVerifyNetPayTransaction(string message) : base(message)
        {
        }

        public CannotVerifyNetPayTransaction(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotVerifyNetPayTransaction(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}