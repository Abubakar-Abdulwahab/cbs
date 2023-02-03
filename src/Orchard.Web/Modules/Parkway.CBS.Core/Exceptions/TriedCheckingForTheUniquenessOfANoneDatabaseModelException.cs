using System;
using System.Runtime.Serialization;

namespace Parkway.CBS.Core.Exceptions
{
    [Serializable]
    public class TriedCheckingForTheUniquenessOfANoneDatabaseModelException : Exception
    {
        public TriedCheckingForTheUniquenessOfANoneDatabaseModelException()
        {
        }

        public TriedCheckingForTheUniquenessOfANoneDatabaseModelException(string message) : base(message)
        {
        }

        public TriedCheckingForTheUniquenessOfANoneDatabaseModelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TriedCheckingForTheUniquenessOfANoneDatabaseModelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
