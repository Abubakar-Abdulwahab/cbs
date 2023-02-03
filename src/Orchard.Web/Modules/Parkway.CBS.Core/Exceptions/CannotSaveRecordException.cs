using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    internal class CannotSaveRecordException : Exception
    {
        public CannotSaveRecordException()
        {
        }

        public CannotSaveRecordException(string message) : base(message)
        {
        }

        public CannotSaveRecordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotSaveRecordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}