using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NoCategoryFoundException : Exception
    {
        public NoCategoryFoundException()
        {
        }

        public NoCategoryFoundException(string message) : base(message)
        {
        }

        public NoCategoryFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoCategoryFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class CouldNotSaveDirectAssessmentBatchRecordException : Exception
    {
        public CouldNotSaveDirectAssessmentBatchRecordException()
        {
        }

        public CouldNotSaveDirectAssessmentBatchRecordException(string message) : base(message)
        {
        }

        public CouldNotSaveDirectAssessmentBatchRecordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveDirectAssessmentBatchRecordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}