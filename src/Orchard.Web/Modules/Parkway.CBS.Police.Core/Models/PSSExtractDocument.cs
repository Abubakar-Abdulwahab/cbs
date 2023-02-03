using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSExtractDocument : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual ExtractDetails ExtractDetails { get; set; }

        public virtual string CommandName { get; set; }

        public virtual string CommandStateName { get; set; }

        public virtual DateTime ApprovalDate { get; set; }

        public virtual string ApprovalNumber { get; set; }

        public virtual string DiarySerialNumber { get; set; }

        public virtual DateTime IncidenDateAndTime { get; set; }

        public virtual string CrossRef { get; set; }

        public virtual string ExtractCategories { get; set; }

        public virtual string Content { get; set; }

        public virtual string DPOName { get; set; }

        public virtual string DPOSignatureContentType { get; set; }

        public virtual string DPOSignatureBlob { get; set; }

        public virtual string ExtractDocumentTemplate { get; set; }

        public virtual string DPORankCode { get; set; }

    }
}