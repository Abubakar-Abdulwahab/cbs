using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class TenantNotFoundException : Exception
    {
        public TenantNotFoundException()
        {
        }

        public TenantNotFoundException(string message) : base(message)
        {
        }

        public TenantNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TenantNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}