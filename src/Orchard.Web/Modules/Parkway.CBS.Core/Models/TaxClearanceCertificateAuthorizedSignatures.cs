using Orchard.Users.Models;
using System;

namespace Parkway.CBS.Core.Models
{
    public class TaxClearanceCertificateAuthorizedSignatures : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        /// <summary>
        /// <see cref="TCCAuthorizedSignatories"/>
        /// </summary>
        public virtual int TCCAuthorizedSignatoryId { get; set; }

        public virtual string OriginalFileName { get; set; }

        public virtual string FilePath { get; set; }

        public virtual string ContentType { get; set; }

        public virtual string TCCAuthorizedSignatureBlob { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual UserPartRecord AddedBy { get; set; } 

        public virtual UserPartRecord Owner { get; set; }

    }
}