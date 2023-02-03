using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NoRecordFoundException : Exception
    {
        public NoRecordFoundException()
        {
        }

        public NoRecordFoundException(string message) : base(message)
        {
        }

        public NoRecordFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoRecordFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}