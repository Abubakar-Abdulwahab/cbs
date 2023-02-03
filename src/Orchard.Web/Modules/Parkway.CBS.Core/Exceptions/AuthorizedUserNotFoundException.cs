using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class AuthorizedUserNotFoundException : Exception
    {
        public AuthorizedUserNotFoundException()
        {
        }

        public AuthorizedUserNotFoundException(string message) : base(message)
        {
        }

        public AuthorizedUserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AuthorizedUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}