using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class DependenciesAreDisabledException : Exception
    {
        public DependenciesAreDisabledException()
        {
        }

        public DependenciesAreDisabledException(string message) : base(message)
        {
        }

        public DependenciesAreDisabledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DependenciesAreDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}