using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;


namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotGeneratePINsException : Exception
    {
        public CouldNotGeneratePINsException()
        {
        }

        public CouldNotGeneratePINsException(string message) : base(message)
        {
        }

        public CouldNotGeneratePINsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotGeneratePINsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}