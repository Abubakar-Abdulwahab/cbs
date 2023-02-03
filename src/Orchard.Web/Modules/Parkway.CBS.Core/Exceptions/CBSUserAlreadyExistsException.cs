using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    public class CBSUserAlreadyExistsException : Exception
    {
        public CBSUserAlreadyExistsException()
        {
        }

        public CBSUserAlreadyExistsException(string message) : base(message)
        {
        }

        public CBSUserAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CBSUserAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}