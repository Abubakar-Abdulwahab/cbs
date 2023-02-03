using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotConnectToCashFlowException : Exception
    {
        public CannotConnectToCashFlowException()
        {
        }

        public CannotConnectToCashFlowException(string message) : base(message)
        {
        }

        public CannotConnectToCashFlowException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotConnectToCashFlowException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}