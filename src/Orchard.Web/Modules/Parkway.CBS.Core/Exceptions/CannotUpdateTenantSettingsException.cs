using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    public class CannotUpdateTenantSettingsException : Exception
    {
        public CannotUpdateTenantSettingsException()
        {
        }

        public CannotUpdateTenantSettingsException(string message) : base(message)
        {
        }

        public CannotUpdateTenantSettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotUpdateTenantSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}