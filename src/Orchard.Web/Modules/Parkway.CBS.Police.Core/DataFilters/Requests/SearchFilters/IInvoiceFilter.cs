using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters
{
    public interface IInvoiceFilter : IDependency
    {

        /// <summary>
        /// Get report view for when an invoice number is being searched for
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>dynamic</returns>
        dynamic GetRequestReportInvoiceVM(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions);

    }
}
