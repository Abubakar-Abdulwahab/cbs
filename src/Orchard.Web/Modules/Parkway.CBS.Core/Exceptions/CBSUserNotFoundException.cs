using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    public class CBSUserNotFoundException : Exception
    {
        public CBSUserNotFoundException()
        {
        }

        public CBSUserNotFoundException(string message) : base(message)
        {
        }

        public CBSUserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CBSUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class CouldNotSaveCBSUserException : Exception
    {
        public CouldNotSaveCBSUserException()
        {
        }

        public CouldNotSaveCBSUserException(string message) : base(message)
        {
        }

        public CouldNotSaveCBSUserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveCBSUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
