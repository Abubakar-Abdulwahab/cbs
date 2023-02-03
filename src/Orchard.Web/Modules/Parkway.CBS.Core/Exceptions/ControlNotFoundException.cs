using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class ControlNotFoundException : Exception
    {
        public ControlNotFoundException()
        {
        }

        public ControlNotFoundException(string message) : base(message)
        {
        }

        public ControlNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ControlNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
