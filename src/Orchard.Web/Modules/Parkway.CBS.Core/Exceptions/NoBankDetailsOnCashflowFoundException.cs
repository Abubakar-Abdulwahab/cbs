using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NoBankDetailsOnCashflowFoundException : Exception
    {
        public NoBankDetailsOnCashflowFoundException()
        {
        }

        public NoBankDetailsOnCashflowFoundException(string message) : base(message)
        {
        }

        public NoBankDetailsOnCashflowFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoBankDetailsOnCashflowFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}