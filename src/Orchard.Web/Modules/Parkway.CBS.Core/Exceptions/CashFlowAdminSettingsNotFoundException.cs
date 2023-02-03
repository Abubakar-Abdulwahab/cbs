using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CashFlowAdminSettingsNotFoundException : Exception
    {
        public CashFlowAdminSettingsNotFoundException()
        {
        }

        public CashFlowAdminSettingsNotFoundException(string message) : base(message)
        {
        }

        public CashFlowAdminSettingsNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CashFlowAdminSettingsNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}