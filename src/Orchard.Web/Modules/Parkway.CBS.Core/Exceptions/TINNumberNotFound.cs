using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class TINNotFoundException : Exception
    {
        public TINNotFoundException()
        {
        }

        public TINNotFoundException(string message) : base(message)
        {
        }

        public TINNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TINNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}