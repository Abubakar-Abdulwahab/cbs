using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class ExtractRequestFiles : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual ExtractDetails ExtractDetails { get; set; }

        public virtual string FileName { get; set; }

        public virtual string FilePath { get; set; }

        public virtual string ContentType { get; set; }

        public virtual string Blob { get; set; }
    }
}