using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxPayerEnumerationReportVM
    {
        public int PageSize { get; set; }

        public IEnumerable<TaxPayerEnumerationLine> LineItems { get; set; }

        public FileUploadReport EnumerationItemsExcelReport { get; set; }
    }
}