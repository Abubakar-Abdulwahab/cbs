using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    public class CannotSaveTenantSettings : Exception
    {
        public CannotSaveTenantSettings()
        {
        }

        public CannotSaveTenantSettings(string message) : base(message)
        {
        }

        public CannotSaveTenantSettings(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotSaveTenantSettings(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
