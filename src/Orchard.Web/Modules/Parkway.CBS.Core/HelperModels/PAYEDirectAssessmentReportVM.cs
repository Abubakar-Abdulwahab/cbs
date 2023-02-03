using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class DirectAssessmentReportItemsVM
    {
        /// <summary>
        /// AssessedBy
        /// </summary>
        public string AssessedBy { get; set; }

        /// <summary>
        /// Assessment Date
        /// </summary>
        public DateTime AssessmentDate { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Assessment Type
        /// </summary>

        public string DirectAssessmentType { get; set; }

        /// <summary>
        /// Tax period month
        /// </summary>
        public  int Month { get; set; }

        /// <summary>
        /// Tax period year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Invoice Amount
        /// </summary>
        public decimal InvoiceAmount { get; set; }

        /// <summary>
        /// Invoice Number
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// Invoice Status
        /// </summary>
        public int InvoiceStatusId { get; set; }

        public string InvoiceStatusDescription { get => ((InvoiceStatus)InvoiceStatusId).ToDescription(); }

        /// <summary>
        /// Payer Name
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// State TIN
        /// </summary>
        public string StateTIN { get; set; }

        /// <summary>
        /// Duration of the Tax period
        /// </summary>
        public string TaxPeriod
        {
            get
            {
                
                return new DateTime(Year, Month, 1).ToString("MMM yyyy");
            }
            
        }
    }

    public class PAYEDirectAssessmentReportVM
    {
        public PAYEDirectAssessmentReportVM()
        {
            DirectAssessmentReportItems = new List<DirectAssessmentReportItemsVM>();
            DirectAssessmentTypes = new List<PAYEDirectAssessmentTypeVM>();
        }

        public int DataSize { get; set; }

        public IEnumerable<DirectAssessmentReportItemsVM> DirectAssessmentReportItems { get; set; }

        public IEnumerable<PAYEDirectAssessmentTypeVM> DirectAssessmentTypes { get; set; }

        public string DirectAssessmentType { get; set; } = "0";

        public string End { get; set; }

        public string From { get; set; }

        public string InvoiceNo { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; } = InvoiceStatus.All;

        public dynamic Pager { get; set; }

        public string TIN { get; set; }

        public decimal TotalAmount { get; set; }
        public string LogoURL { get; set; }
        public string TenantName { get; set; }
    }
}