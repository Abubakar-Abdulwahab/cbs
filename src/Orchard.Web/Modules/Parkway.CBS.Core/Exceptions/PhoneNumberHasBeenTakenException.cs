using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    public class PhoneNumberHasBeenTakenException : Exception
    {
        public PhoneNumberHasBeenTakenException()
        {
        }

        public PhoneNumberHasBeenTakenException(string message) : base(message)
        {
        }

        public PhoneNumberHasBeenTakenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PhoneNumberHasBeenTakenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}