using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    public class TenantStateHasAlreadyBeenSetException : Exception
    {
        public TenantStateHasAlreadyBeenSetException()
        {
        }

        public TenantStateHasAlreadyBeenSetException(string message) : base(message)
        {
        }

        public TenantStateHasAlreadyBeenSetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TenantStateHasAlreadyBeenSetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class IncorrectIdentifierFormatException : Exception
    {
        public IncorrectIdentifierFormatException()
        {
        }

        public IncorrectIdentifierFormatException(string message) : base(message)
        {
        }

        public IncorrectIdentifierFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IncorrectIdentifierFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}