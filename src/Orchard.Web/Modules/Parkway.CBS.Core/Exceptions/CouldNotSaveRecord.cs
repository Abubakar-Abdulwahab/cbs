using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotSaveRecord : Exception
    {
        public CouldNotSaveRecord()
        {
        }

        public CouldNotSaveRecord(string message) : base(message)
        {
        }

        public CouldNotSaveRecord(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveRecord(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}