using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    internal class NoScheduleStatusFoundException : Exception
    {
        public NoScheduleStatusFoundException()
        {
        }

        public NoScheduleStatusFoundException(string message) : base(message)
        {
        }

        public NoScheduleStatusFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoScheduleStatusFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}