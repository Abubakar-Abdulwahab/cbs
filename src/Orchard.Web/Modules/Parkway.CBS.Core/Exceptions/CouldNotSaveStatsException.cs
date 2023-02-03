using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotSaveStatsException : Exception
    {
        public CouldNotSaveStatsException()
        {
        }

        public CouldNotSaveStatsException(string message) : base(message)
        {
        }

        public CouldNotSaveStatsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveStatsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}