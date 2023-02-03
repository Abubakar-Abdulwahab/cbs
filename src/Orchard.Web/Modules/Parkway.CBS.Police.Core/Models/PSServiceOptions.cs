using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSServiceOptions : CBSModel
    {
        public virtual PSService Service { get; set; }

        public virtual PSService ServiceOption { get; set; }

        public virtual string Name { get; set; }

        /// <summary>
        /// <para cref="Enums.CharacterCertificateOption">An example of this would be 
        /// <see cref="Enums.CharacterCertificateOption"/> for character certificate 
        /// or a different value for Escort and Guards</para>
        /// </summary>
        public virtual string ServiceOptionType { get; set; }

        public virtual int ServiceOptionTypeId { get; set; }

        public virtual bool IsActive { get; set; }
    }
}