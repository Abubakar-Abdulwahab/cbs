using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class TCCRequestVM
    {
        public int ExemptionTypeValue { private get; set; }

        public DateTime RequestDateValue { private get; set; }

        public bool IsRentedApartmentValue { private get; set; }

        public int StatusValue { private get; set; }

        public Int64 Id { get; set; }

        public string ApplicantName { get; set; }

        public string Status
        {
            get { return ((TCCRequestStatus)this.StatusValue).ToDescription(); }
        }

        public string ResidentialAddress { get; set; }

        public string OfficeAddress { get; set; }

        public string RequestDate
        {
            get { return this.RequestDateValue.ToString("dd MMM yyyy HH:mm"); }
        }

        public string PayerId { get; set; }

        public string IsRentedApartment
        {
            get { return this.IsRentedApartmentValue ? "YES" : "NO"; }
        }

        public string IsExempted
        {
            get
            {
                return ((TCCExemptionType)this.ExemptionTypeValue) != TCCExemptionType.None ? "YES" : "NO";
            }
        }

        public DateTime LastActionDate { get; set; }

        public string RequestReason { get; set; }

        public string ApplicationNumber { get; set; }

        public string ApprovedBy { get; set; }

        public string Comment { get; set; }

        public string ExemptionType
        {
            get { return ((TCCExemptionType)this.ExemptionTypeValue).ToDescription(); }
        }

        public string TCCNumber { get; set; }
    }
}