using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    public class LGANotFoundException : Exception
    {
        public LGANotFoundException()
        {
        }

        public LGANotFoundException(string message) : base(message)
        {
        }

        public LGANotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LGANotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}