using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public  class NoInvoicesMatchingTheParametersFoundException : Exception
    {
        public NoInvoicesMatchingTheParametersFoundException()
        {
        }

        public NoInvoicesMatchingTheParametersFoundException(string message) : base(message)
        {
        }

        public NoInvoicesMatchingTheParametersFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoInvoicesMatchingTheParametersFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}