using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class UserNotAuthorizedForThisActionException : Exception
    {
        public UserNotAuthorizedForThisActionException()
        {
        }

        public UserNotAuthorizedForThisActionException(string message) : base(message)
        {
        }

        public UserNotAuthorizedForThisActionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNotAuthorizedForThisActionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
