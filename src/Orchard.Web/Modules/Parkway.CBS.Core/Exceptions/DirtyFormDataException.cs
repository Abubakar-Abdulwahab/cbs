using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class DirtyFormDataException : Exception
    {
        public DirtyFormDataException()
        {
        }

        public DirtyFormDataException(string message) : base(message)
        {
        }

        public DirtyFormDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DirtyFormDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class APIPermissionException : Exception
    {
        public APIPermissionException()
        {
        }

        public APIPermissionException(string message) : base(message)
        {
        }

        public APIPermissionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected APIPermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
