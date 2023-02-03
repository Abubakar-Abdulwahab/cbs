using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.OSGOF.Admin.ViewModels
{

    public class CellSiteReportQueryObj
    {
        public IEnumerable<CellSiteReturnModelVM> CellSites { get; set; }

        public int RecordsWithErrors { get; set; }

        public int RecordsWithoutErrors { get; set; }
        public decimal TotalAmount { get; set; }
    }



    public class CellSiteReportVM
    {
        public int PageSize { get; set; }

        public string Amount { get; set; }

        public List<CellSiteReturnModelVM> CellSites { get; set; }

        public FileUploadReport CellSitesReport { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string Category { get; set; }

        public string RevenueHeadName { get; set; }

        public string MDAName { get; set; }

        public string Recipient { get; set; }

        public string TIN { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string AdapterValue { get; set; }

        public bool DoWork { get; set; }

        /// <summary>
        /// Do comparison leg
        /// </summary>
        public bool DoLeg2 { get; set; }

        public string ErrorMessage { get; set; }
    }
}