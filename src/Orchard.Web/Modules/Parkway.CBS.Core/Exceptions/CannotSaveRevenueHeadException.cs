using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotSaveRevenueHeadException : Exception
    {
        public CannotSaveRevenueHeadException()
        {
        }

        public CannotSaveRevenueHeadException(string message) : base(message)
        {
        }

        public CannotSaveRevenueHeadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotSaveRevenueHeadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
