using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class PartPaymentNotAllowedException : Exception
    {
        public PartPaymentNotAllowedException()
        {

        }

        public PartPaymentNotAllowedException(string message) : base(message)
        {

        }
    }
}