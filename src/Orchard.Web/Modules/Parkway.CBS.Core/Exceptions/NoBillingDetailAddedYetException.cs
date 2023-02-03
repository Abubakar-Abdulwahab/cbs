using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NoBillingDetailAddedYetException : Exception
    {
        public NoBillingDetailAddedYetException()
        {
        }

        public NoBillingDetailAddedYetException(string message) : base(message)
        {
        }

        public NoBillingDetailAddedYetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoBillingDetailAddedYetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
