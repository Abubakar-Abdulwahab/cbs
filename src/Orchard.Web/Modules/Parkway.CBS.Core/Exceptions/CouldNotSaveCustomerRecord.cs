using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotSaveCustomerRecord : Exception
    {
        public CouldNotSaveCustomerRecord()
        {
        }

        public CouldNotSaveCustomerRecord(string message) : base(message)
        {
        }

        public CouldNotSaveCustomerRecord(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveCustomerRecord(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}