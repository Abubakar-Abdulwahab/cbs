using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEReceiptSearchParams
    {
        public long TaxEntityId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string ReceiptNumber { get; set; }

        public string datefilter { get; set; }

        public bool DontPageData { get; set; }

        public bool IsEmployer { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        /// <summary>
        /// StateTIN
        /// </summary>
        public string PayerId { get; set; }

        public PAYEReceiptUtilizationStatus Status { get; set; }

    }
}