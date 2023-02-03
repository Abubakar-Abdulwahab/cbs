using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NoSearchFilterExceptionFound : Exception
    {
        public NoSearchFilterExceptionFound()
        {
        }

        public NoSearchFilterExceptionFound(string message) : base(message)
        {
        }

        public NoSearchFilterExceptionFound(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoSearchFilterExceptionFound(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}