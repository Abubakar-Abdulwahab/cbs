using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class TCCRequestDetailVM
    {
        public Int64 Id { get; set; }

        public string ApplicantName { get; set; }

        public TCCRequestStatus Status { get; set; }

        public string PhoneNumber { get; set; }

        public string ResidentialAddress { get; set; }

        public string OfficeAddress { get; set; }

        public string LandlordName { get; set; }

        public string LandlordAddress { get; set; }

        public string RequestDate { get; set; }

        public string TIN { get; set; }

        public bool IsRentedApartment { get; set; }

        public DateTime? LastActionDate { get; set; }

        public string RequestReason { get; set; }

        public TCCExemptionType ExemptionType { get; set; }

        public string ExemptionTypeS { get; set; }

        public string Occupation { get; set; }

        public string ApprovedBy { get; set; }

        public string Comment { get; set; }

        public string HusbandName { get; set; }

        /// <summary>
        /// For exempted wife
        /// </summary>
        public string HusbandAddress { get; set; }

        /// <summary>
        /// For exempted student
        /// </summary>
        public string InstitutionName { get; set; }

        /// <summary>
        /// For exempted student
        /// </summary>
        public string IdentificationNumber { get; set; }

        public string ApplicationNumber { get; set; }

        public string DevelopmentLevyInvoiceNumber { get; set; }

        public IEnumerable<TaxPaymentDetailVM> Payments { get; set; }

        public List<TCCRequestAttachmentVM> Attachments { get; set; }

        public int ApprovalStatus { get; set; }

        public int ApproverId { get; set; }

        public string TCCNumber { get; set; }

        public string BarCodeSavingPath { get; set; }

        public HeaderObj HeaderObj { get; set; }

        /// <summary>
        /// Year the TCC is being requested for
        /// </summary>
        public int ApplicationYear { get; set; }

        public long TaxEntityId { get; set; }

        public int ApprovalStatusLevelId { get; set; }

    }
}