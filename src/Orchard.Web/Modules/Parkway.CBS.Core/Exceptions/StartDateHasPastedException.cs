using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class StartDateHasPassedException : Exception
    {
        public StartDateHasPassedException()
        {
        }

        public StartDateHasPassedException(string message) : base(message)
        {
        }

        public StartDateHasPassedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StartDateHasPassedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}