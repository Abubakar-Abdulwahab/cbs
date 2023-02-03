using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class SectorNotFoundException : Exception
    {
        public SectorNotFoundException()
        {
        }

        public SectorNotFoundException(string message) : base(message)
        {
        }

        public SectorNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SectorNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}