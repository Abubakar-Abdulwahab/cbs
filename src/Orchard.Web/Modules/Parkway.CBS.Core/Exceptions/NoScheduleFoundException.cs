using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    internal class NoScheduleFoundException : Exception
    {
        public NoScheduleFoundException()
        {
        }

        public NoScheduleFoundException(string message) : base(message)
        {
        }

        public NoScheduleFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoScheduleFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}