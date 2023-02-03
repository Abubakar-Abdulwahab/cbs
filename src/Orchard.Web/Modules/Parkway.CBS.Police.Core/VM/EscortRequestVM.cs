using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.VM
{
    public class EscortRequestVM : RequestDumpVM
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int NumberOfOfficers { get; set; }

        public PSBillingType PSBillingType { get; set; }

        public int PSBillingTypeDurationNumber { get; set; }

        public string Address { get; set; }

        public string FileRefNumber { get; set; }

        public int Status { get; set; }

        public DateTime ParsedStartDate { get; set; }

        public DateTime ParsedEndDate { get; set; }

        public short FormErrorNumber { get; set; }

        public int DurationNumber { get; set; }

        public int DurationType { get; set; }

        public int SelectedReason { get; set; }

        public ICollection<PSSReasonVM> Reasons { get; set; }

        public bool OfficersHasBeenAssigned { get; set; }

        public IEnumerable<ProposedEscortOffficerVM> ProposedOfficers { get; set; }

        public int SubCategoryId { get; set; }

        public int SubSubCategoryId { get; set; }

        public string ApprovalNumber { get; set; }

        public IEnumerable<PSSEscortServiceCategoryVM> EscortServiceCategories { get; set; }

        public IEnumerable<PSSEscortServiceCategoryVM> EscortCategoryTypes { get; set; }

        public IEnumerable<int> SelectedEscortServiceCategories { get; set; }

        public int SelectedOriginState { get; set; }

        public string OriginStateName { get; set; }

        public int SelectedOriginLGA { get; set; }

        public string OriginLGAName { get; set; }

        public List<LGA> OriginLGAs { get; set; }

        public string AddressOfOriginLocation{ get; set; }

        public bool ShowExtraFieldsForServiceCategoryType { get; set; }

        public string TaxEntitySubSubCategoryName { get; set; }

        public string ServiceCategoryName { get; set; }

        public string ServiceCategoryTypeName { get; set; }

        public IEnumerable<CommandTypeVM> CommandTypes { get; set; }

        public int SelectedCommandType { get; set; }

        public string SelectedCommandTypeName { get; set; }

        public IEnumerable<CommandVM> TacticalSquads { get; set; }

        public int SelectedTacticalSquad { get; set; }

        public IEnumerable<CommandVM> Formations { get; set; }

        public string SelectedFormationName { get; set; }

        public bool ViewedTermsAndConditionsModal { get; set; }

        public PSServiceCaveatVM Caveat { get; set; }
    }
}