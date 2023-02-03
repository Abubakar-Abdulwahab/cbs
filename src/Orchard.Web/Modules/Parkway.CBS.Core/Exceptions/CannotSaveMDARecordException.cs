using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotSaveMDARecordException : Exception
    {
        public CannotSaveMDARecordException()
        {
        }

        public CannotSaveMDARecordException(string message) : base(message)
        {
        }

        public CannotSaveMDARecordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotSaveMDARecordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
