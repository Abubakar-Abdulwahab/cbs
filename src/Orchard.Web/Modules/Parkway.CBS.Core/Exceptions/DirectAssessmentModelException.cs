using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class DirectAssessmentModelException : Exception
    {
        public DirectAssessmentModelException()
        {
        }

        public DirectAssessmentModelException(string message) : base(message)
        {
        }

        public DirectAssessmentModelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DirectAssessmentModelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}