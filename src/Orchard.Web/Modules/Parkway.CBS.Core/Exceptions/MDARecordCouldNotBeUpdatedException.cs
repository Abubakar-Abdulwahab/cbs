using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class MDARecordCouldNotBeUpdatedException : Exception
    {
        public MDARecordCouldNotBeUpdatedException()
        {
        }

        public MDARecordCouldNotBeUpdatedException(string message) : base(message)
        {
        }

        public MDARecordCouldNotBeUpdatedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MDARecordCouldNotBeUpdatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
