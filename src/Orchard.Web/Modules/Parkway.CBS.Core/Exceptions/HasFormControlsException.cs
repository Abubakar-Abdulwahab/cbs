using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class HasFormControlsException : Exception
    {
        public HasFormControlsException()
        {
        }

        public HasFormControlsException(string message) : base(message)
        {
        }

        public HasFormControlsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HasFormControlsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}