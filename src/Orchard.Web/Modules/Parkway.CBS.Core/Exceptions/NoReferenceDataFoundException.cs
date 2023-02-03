using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    public class NoReferenceDataFoundException : Exception
    {
        public NoReferenceDataFoundException()
        {
        }

        public NoReferenceDataFoundException(string message) : base(message)
        {
        }

        public NoReferenceDataFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoReferenceDataFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
