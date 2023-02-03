using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class RecordAlreadyExistsException : Exception
    {
        public RecordAlreadyExistsException()
        {
        }

        public RecordAlreadyExistsException(string message) : base(message)
        {
        }

        public RecordAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RecordAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}