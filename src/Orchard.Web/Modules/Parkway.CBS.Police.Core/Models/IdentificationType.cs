using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class IdentificationType : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual bool HasIntegration { get; set; }

        public virtual string ImplementationClass { get; set; }

        public virtual string ImplementingClassName { get; set; }

        public virtual string Hint { get; set; }

        /// <summary>
        /// Flag to show that the identification type has biometric support
        /// </summary>
        public virtual bool HasBiometricSupport { get; set; }

    }
}