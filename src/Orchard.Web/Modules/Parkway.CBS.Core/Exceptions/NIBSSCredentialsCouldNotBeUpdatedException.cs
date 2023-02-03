using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class NIBSSCredentialsCouldNotBeUpdatedException : Exception
    {
        public NIBSSCredentialsCouldNotBeUpdatedException()
        {
        }

        public NIBSSCredentialsCouldNotBeUpdatedException(string message) : base(message)
        {
        }

        public NIBSSCredentialsCouldNotBeUpdatedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NIBSSCredentialsCouldNotBeUpdatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}