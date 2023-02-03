using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class IdentificationTypeFileAttachment : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual IdentificationType IdentificationType { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual string OriginalFileName { get; set; }

        public virtual string FilePath { get; set; }

        public virtual string ContentType { get; set; }

        public virtual string Blob { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}