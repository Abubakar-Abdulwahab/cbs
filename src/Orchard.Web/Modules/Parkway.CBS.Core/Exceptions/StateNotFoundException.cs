using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    public class StateNotFoundException : Exception
    {
        public StateNotFoundException()
        {
        }

        public StateNotFoundException(string message) : base(message)
        {
        }

        public StateNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StateNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}