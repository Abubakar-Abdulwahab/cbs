using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class MDARecordNotFoundException : Exception
    {
        public MDARecordNotFoundException()
        {
        }

        public MDARecordNotFoundException(string message) : base(message)
        {
        }

        public MDARecordNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MDARecordNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }


    [Serializable]
    public class AmountTooSmallException : Exception
    {
        public AmountTooSmallException()
        {
        }

        public AmountTooSmallException(string message) : base(message)
        {
        }

        public AmountTooSmallException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AmountTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
