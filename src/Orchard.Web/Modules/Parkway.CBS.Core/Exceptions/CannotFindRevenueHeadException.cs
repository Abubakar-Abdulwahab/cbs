using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotFindRevenueHeadException : Exception
    {
        public CannotFindRevenueHeadException()
        {
        }

        public CannotFindRevenueHeadException(string message) : base(message)
        {
        }

        public CannotFindRevenueHeadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotFindRevenueHeadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
