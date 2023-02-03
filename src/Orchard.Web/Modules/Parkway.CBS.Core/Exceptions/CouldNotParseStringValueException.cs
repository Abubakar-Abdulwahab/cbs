using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotParseStringValueException : Exception
    {
        public CouldNotParseStringValueException()
        {
        }

        public CouldNotParseStringValueException(string message) : base(message)
        {
        }

        public CouldNotParseStringValueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotParseStringValueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


    [Serializable]
    public class CannotFindTenantIdentifierException : Exception
    {
        public CannotFindTenantIdentifierException()
        {
        }

        public CannotFindTenantIdentifierException(string message) : base(message)
        {
        }

        public CannotFindTenantIdentifierException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotFindTenantIdentifierException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}