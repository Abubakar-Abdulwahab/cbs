using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class ExtractRequestDetailsVM : PSSRequestDetailsVM
    {
        public int SelectedCategory { get; set; }

        public int SelectedSubCategory { get; set; }

        public string SelectedCategoryName { get; set; }

        public string SelectedSubCategoryName { get; set; }

        public string IsIncidentReported { get; set; }

        public string IncidentReportedDate { get; set; }

        public string AffidavitNumber { get; set; }

        public DateTime? AffidavitDateOfIsssuance { get; set; }

        public List<ExtractRequestAttachmentVM> Attachments { get; set; }

        public IEnumerable<PSSRequestExtractDetailsCategoryVM> SelectedExtractCategories { get; set; }

        public string Details { get; set; }

        public string Action { get; set; }

        public string DiarySerialNumber { get; set; }

        public string IncidentDate { get; set; }

        public DateTime? IncidentDateAndTimeParsed { get; set; }

        public string IncidentTime { get; set; }

        public string CrossReferencing { get; set; }

        public string Content { get; set; }

        public int DefinitionId { get; set; }

        public int Position { get; set; }

        public bool IsLastApprover { get; set; }

        public ICollection<ProposedEscortOffficerVM> SelectedDPO { get; set; }

    }
}