using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotSaveTaxPayerException : Exception
    {
        public CouldNotSaveTaxPayerException()
        {
        }

        public CouldNotSaveTaxPayerException(string message) : base(message)
        {
        }

        public CouldNotSaveTaxPayerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveTaxPayerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}