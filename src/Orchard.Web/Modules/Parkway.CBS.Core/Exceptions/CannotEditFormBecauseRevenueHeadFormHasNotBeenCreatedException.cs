using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class CannotEditFormBecauseRevenueHeadFormHasNotBeenCreatedException : Exception
    {
        public CannotEditFormBecauseRevenueHeadFormHasNotBeenCreatedException()
        {
        }

        public CannotEditFormBecauseRevenueHeadFormHasNotBeenCreatedException(string message) : base(message)
        {
        }

        public CannotEditFormBecauseRevenueHeadFormHasNotBeenCreatedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotEditFormBecauseRevenueHeadFormHasNotBeenCreatedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
