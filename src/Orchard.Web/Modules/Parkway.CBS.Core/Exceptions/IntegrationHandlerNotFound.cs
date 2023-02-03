using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class IntegrationHandlerNotFound : Exception
    {
        public IntegrationHandlerNotFound()
        {
        }

        public IntegrationHandlerNotFound(string message) : base(message)
        {
        }

        public IntegrationHandlerNotFound(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IntegrationHandlerNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}