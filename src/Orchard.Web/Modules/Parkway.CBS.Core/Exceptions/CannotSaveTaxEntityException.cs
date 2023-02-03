using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotSaveTaxEntityException : Exception
    {
        public CannotSaveTaxEntityException()
        {
        }

        public CannotSaveTaxEntityException(string message) : base(message)
        {
        }

        public CannotSaveTaxEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotSaveTaxEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}