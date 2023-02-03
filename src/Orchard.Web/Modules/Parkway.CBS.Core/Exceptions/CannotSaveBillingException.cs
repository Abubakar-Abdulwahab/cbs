using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotSaveBillingException :Exception
    {
        public CouldNotSaveBillingException()
        {
        }

        public CouldNotSaveBillingException(string message) : base(message)
        {
        }

        public CouldNotSaveBillingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveBillingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
