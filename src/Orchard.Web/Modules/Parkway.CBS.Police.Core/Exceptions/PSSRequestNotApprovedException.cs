using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Police.Core.Exceptions
{
    [Serializable]
    public class PSSRequestNotApprovedException : Exception
    {
        public PSSRequestNotApprovedException()
        {
        }

        public PSSRequestNotApprovedException(string message) : base(message)
        {
        }

        public PSSRequestNotApprovedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PSSRequestNotApprovedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}