using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceAssessmentSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string SMDA { get; set; }

        public int MDAId { get; set; }

        public string SRevenueHeadId { get; set; }

        public int RevenueHeadId { get; set; }

        public PaymentOptions Options { get; set; }

        public string InvoiceNumber { get; set; }

        public string SCategory { get; set; }

        public int Category { get; set; }

        public int Take { get; set; }

        public int Skip { get; set; }

        public string ExportFormat { get; set; }

        public string ApplicantName { get; set; }

        public string ApplicantCategory { get; set; }

        public string HasPaid { get; set; }

        public string TIN { get; set; }

        public string RCNumber { get; set; }

        public Direction Direction { get; set; }

        public FilterDate DateFilterBy { get; set; }

        public int AdminUserId { get; set; }

        public bool DontPageData { get; set; }
    }
}