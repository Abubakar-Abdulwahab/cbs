using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class DateTimeCouldNotBeParsedException : Exception
    {
        public DateTimeCouldNotBeParsedException()
        {
        }

        public DateTimeCouldNotBeParsedException(string message) : base(message)
        {
        }

        public DateTimeCouldNotBeParsedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DateTimeCouldNotBeParsedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}