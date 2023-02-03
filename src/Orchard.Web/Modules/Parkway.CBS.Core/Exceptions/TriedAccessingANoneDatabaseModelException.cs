using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class TriedAccessingANoneDatabaseModelException : Exception
    {
        public TriedAccessingANoneDatabaseModelException()
        {
        }

        public TriedAccessingANoneDatabaseModelException(string message) : base(message)
        {
        }

        public TriedAccessingANoneDatabaseModelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TriedAccessingANoneDatabaseModelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
