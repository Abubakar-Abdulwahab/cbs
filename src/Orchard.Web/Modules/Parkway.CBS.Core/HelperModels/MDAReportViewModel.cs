using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class MDAReportViewModel
    {
        public dynamic Pager { get; set; }

        public IEnumerable<DetailReport> ReportRecords { get; set; }

        public FilterDate DateFilterBy { get; set; }

        public string FromRange { get; set; }

        public string EndRange { get; set; }

        public string MDAName { get; set; }

        public string RevenueHeadName { get; set; }

        public string SelectedMDA { get; set; }

        public string SelectedRevenueHead { get; set; }

        public IEnumerable<MDAVM> MDAs { get; set; }

        public IEnumerable<RevenueHeadDropDownListViewModel> RevenueHeads { get; set; }

        public PaymentOptions Options { get; set; }

        public decimal TotalInvoiceAmount { get; set; }

        public Int64 TotalNumberOfInvoicesSent { get; set; }


        public TaxPayerType CustomerType { get; set; }

        public List<TaxEntityCategory> Sectors { get; set; }

        public IEnumerable<TaxEntityCategoryVM> Categories { get; set; }

        public string SectorSelected { get; set; }

        public string CategoryName { get; set; }

        /// <summary>
        /// Logo URL
        /// </summary>
        public string LogoURL { get; set; }

        /// <summary>
        /// Tenant name
        /// </summary>
        public string TenantName { get; set; }

        public string Token { get; set; }
    }    
}