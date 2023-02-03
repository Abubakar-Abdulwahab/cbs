using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NoFrequencyTypeFoundException : Exception
    {
        public NoFrequencyTypeFoundException()
        {
        }

        public NoFrequencyTypeFoundException(string message) : base(message)
        {
        }

        public NoFrequencyTypeFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoFrequencyTypeFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}