using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class ExtractDocumentVM
    {
        public long ExtractDetailsId { get; set; }

        public string CommandName { get; set; }

        public string CommandStateName { get; set; }

        public DateTime ApprovalDate { get; set; }

        public string ApprovalDateString => ApprovalDate.ToString("dd/MM/yyyy");

        public string ApprovalNumber { get; set; }

        public string DiarySerialNumber { get; set; }

        public string IncidentDate { get; set; }

        public string IncidentTime { get; set; }

        public DateTime? IncidenDateAndTimeParsed { get; set; }

        public string CrossRef { get; set; }

        public IEnumerable<string> ExtractCategories { get; set; }

        public string ExtractCategoriesConcat { get; set; }

        public string Content { get; set; }

        public string DPOName { get; set; }

        public string LogoURL { get; set; }

        public string PossapLogoUrl { get; set; }

        public string PSSExtractDocumentBGPath { get; set; }

        public string PSSExtractDocumentStripURL { get; set; }

        public string Template { get; set; }

        public string ValidateDocumentUrl { get; set; }

        public string DPORankCode { get; set; }

        public string DPOSignatureBlob { get; set; }

        public string DPOSignatureContentType { get; set; }

        public string DPOSignatureImageSrc { get; set; }
    }
}