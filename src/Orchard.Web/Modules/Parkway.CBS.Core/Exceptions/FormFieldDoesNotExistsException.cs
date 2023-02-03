using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class FormFieldDoesNotExistsException : Exception
    {
        public FormFieldDoesNotExistsException()
        {
        }

        public FormFieldDoesNotExistsException(string message) : base(message)
        {
        }

        public FormFieldDoesNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FormFieldDoesNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}