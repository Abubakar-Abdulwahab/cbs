using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NoDueDateTypeFoundException : Exception
    {
        public NoDueDateTypeFoundException()
        {
        }

        public NoDueDateTypeFoundException(string message) : base(message)
        {
        }

        public NoDueDateTypeFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoDueDateTypeFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}