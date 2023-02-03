using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Police.Core.Exceptions
{
    [Serializable]
    public class PSSRequestNotPendingApprovalException : Exception
    {
        public PSSRequestNotPendingApprovalException()
        {
        }

        public PSSRequestNotPendingApprovalException(string message) : base(message)
        {
        }

        public PSSRequestNotPendingApprovalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PSSRequestNotPendingApprovalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}