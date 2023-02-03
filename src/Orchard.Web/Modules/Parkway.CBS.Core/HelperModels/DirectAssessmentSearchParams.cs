using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class DirectAssessmentSearchParams
    {
        /// <summary>
        /// Assessment Type
        /// </summary>
        public int DirectAssessmentType { get; set; }

        /// <summary>
        /// Search enddate parameter
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Search Invoice Number parameter
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Search Invoice status parameter
        /// </summary>
        public InvoiceStatus InvoiceStatus { get; set; }

        public int Skip { get; set; }

        /// <summary>
        /// Search start date parament
        /// </summary>
        public DateTime StartDate { get; set; }

        public int Take { get; set; }

        /// <summary>
        /// Search TIN parameter
        /// </summary>
        public string TIN { get; set; }
        public bool DontPageData { get; set; }
    }
}