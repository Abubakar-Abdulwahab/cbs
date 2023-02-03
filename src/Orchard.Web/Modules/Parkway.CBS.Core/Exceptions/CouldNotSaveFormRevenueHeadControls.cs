using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CouldNotSaveFormRevenueHeadControls : Exception
    {
        public CouldNotSaveFormRevenueHeadControls()
        {
        }

        public CouldNotSaveFormRevenueHeadControls(string message) : base(message)
        {
        }

        public CouldNotSaveFormRevenueHeadControls(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotSaveFormRevenueHeadControls(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}