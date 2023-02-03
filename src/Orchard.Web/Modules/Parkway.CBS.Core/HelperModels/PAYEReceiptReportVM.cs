using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEReceiptReportVM
    {
        public long TaxEntityId { get; set; }

        public string From { get; set; }

        public string End { get; set; }

        public string ReceiptNumber { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public IEnumerable<PAYEReceiptVM> ReceiptItems { get; set; }

        /// <summary>
        /// StateTIN
        /// </summary>
        public string PayerId { get; set; }

        public PAYEReceiptUtilizationStatus Status { get; set; }

        public dynamic Pager { get; set; }

        public int TotalRequestRecord { get; set; }

        public string LogoURL { get; set; }

        public string TenantName { get; set; }

    }
}