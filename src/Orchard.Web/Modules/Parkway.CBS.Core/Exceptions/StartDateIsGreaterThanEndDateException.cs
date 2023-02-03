using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    public class StartDateIsGreaterThanEndDateException : Exception
    {
        public StartDateIsGreaterThanEndDateException()
        {
        }

        public StartDateIsGreaterThanEndDateException(string message) : base(message)
        {
        }

        public StartDateIsGreaterThanEndDateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected StartDateIsGreaterThanEndDateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}