using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Police.Core.Exceptions
{
    [Serializable]
    public class PSSRequestAlreadyExistsException : Exception
    {
        public PSSRequestAlreadyExistsException()
        {
        }

        public PSSRequestAlreadyExistsException(string message) : base(message)
        {
        }

        public PSSRequestAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PSSRequestAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}