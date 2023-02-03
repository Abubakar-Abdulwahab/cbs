using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class TCCApplicationRequestVM
    {
        [Required(ErrorMessage = "Applicant Name is required")]
        public string ApplicantName { get; set; }

        [Required(ErrorMessage = "Residential Address is required")]
        public string ResidentialAddress { get; set; }

        [Required(ErrorMessage = "Office Address is required")]
        public string OfficeAddress { get; set; }

        public string Occupation { get; set; }

        [StringLength(14, MinimumLength = 6, ErrorMessage = "Add a valid phone number")]
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "State TIN is required")]
        public string StateTIN { get; set; }

        public Int64 TaxEntityId { get; set; }

        public bool IsRentedApartment { get; set; }

        public string LandlordName { get; set; }

        public string LandlordAddress { get; set; }

        [Required(ErrorMessage = "Request Reason is required")]
        public string RequestReason { get; set; }

        [Required(ErrorMessage = "Development Levy Invoice is required")]
        public string DevelopmentLevyInvoice { get; set; }

        public long DevelopmentLevyInvoiceId { get; set; }

        public int ExemptionTypeId { get; set; }

        public string HusbandName { get; set; }

        public string HusbandAddress { get; set; }

        public string InstitutionName { get; set; }

        public string IdCardNumber { get; set; }

        public bool IsCertify { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public string Message { get; set; }

        public int ApplicationYear { get; set; }

        public HeaderObj HeaderObj { get; set; }

    }
}