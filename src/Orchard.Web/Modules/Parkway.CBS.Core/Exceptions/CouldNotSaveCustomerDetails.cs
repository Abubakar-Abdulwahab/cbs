using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotSaveCustomerDetails : Exception
    {
        public CouldNotSaveCustomerDetails()
        {
        }

        public CouldNotSaveCustomerDetails(string message) : base(message)
        {
        }

        public CouldNotSaveCustomerDetails(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveCustomerDetails(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}