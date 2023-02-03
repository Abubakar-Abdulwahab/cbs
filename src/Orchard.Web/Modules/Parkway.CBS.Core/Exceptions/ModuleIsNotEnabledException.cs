using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class ModuleIsNotEnabledException : Exception
    {
        public ModuleIsNotEnabledException()
        {
        }

        public ModuleIsNotEnabledException(string message) : base(message)
        {
        }

        public ModuleIsNotEnabledException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ModuleIsNotEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}