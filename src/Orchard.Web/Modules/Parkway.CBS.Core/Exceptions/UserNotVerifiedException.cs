using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    public class UserNotVerifiedException : Exception
    {
        private string _token = string.Empty;

        public void AddToken(string token)
        {
            _token = token;
        }

        public string GetToken()
        {
            return _token;
        }

        public UserNotVerifiedException()
        {
        }

        public UserNotVerifiedException(string message) : base(message)
        {
        }

        public UserNotVerifiedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UserNotVerifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
