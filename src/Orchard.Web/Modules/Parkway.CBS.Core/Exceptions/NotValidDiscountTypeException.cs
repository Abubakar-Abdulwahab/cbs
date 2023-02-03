using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Parkway.CBS.Core.Exceptions
{
    public class NotValidDiscountTypeException : Exception
    {
        public NotValidDiscountTypeException()
        {
        }

        public NotValidDiscountTypeException(string message) : base(message)
        {
        }

        public NotValidDiscountTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotValidDiscountTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}