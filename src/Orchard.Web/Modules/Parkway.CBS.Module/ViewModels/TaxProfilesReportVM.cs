using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Module.ViewModels
{
    /// <summary>
    /// View model for tax profiles report
    /// </summary>
    public class TaxProfilesReportVM
    {
        public dynamic Pager { get; set; }

        public int TaxCategory { get; set; }

        public IEnumerable<TaxEntityCategoryVM> TaxCategories { get; set; }

        public int NumberOfTaxPayersWithoutTIN { get; set; }
        public int TotalNumberOfTaxPayers { get; set; }
        public int NumberOfIncompleteRecords { get; set; }
        public List<TaxEntityReportDetail> ReportRecords { get; set; }

        public string PayerId { get; set; }

        public string TIN { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }
    }
}