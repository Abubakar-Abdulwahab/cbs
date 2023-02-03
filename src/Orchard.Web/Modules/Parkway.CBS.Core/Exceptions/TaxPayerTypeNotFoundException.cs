using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class TaxPayerTypeNotFoundException : Exception
    {
        public TaxPayerTypeNotFoundException()
        {
        }

        public TaxPayerTypeNotFoundException(string message) : base(message)
        {
        }

        public TaxPayerTypeNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TaxPayerTypeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}