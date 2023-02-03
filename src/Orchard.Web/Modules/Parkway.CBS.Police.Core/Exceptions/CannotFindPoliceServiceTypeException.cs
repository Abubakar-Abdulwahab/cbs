using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Police.Core.Exceptions
{
    [Serializable]
    public class CannotFindPoliceServiceTypeException : Exception
    {
        public CannotFindPoliceServiceTypeException()
        {
        }

        public CannotFindPoliceServiceTypeException(string message) : base(message)
        {
        }

        public CannotFindPoliceServiceTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotFindPoliceServiceTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}