using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotUpdateRevenueHeadException : Exception
    {
        public CannotUpdateRevenueHeadException()
        {
        }

        public CannotUpdateRevenueHeadException(string message) : base(message)
        {
        }

        public CannotUpdateRevenueHeadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotUpdateRevenueHeadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
