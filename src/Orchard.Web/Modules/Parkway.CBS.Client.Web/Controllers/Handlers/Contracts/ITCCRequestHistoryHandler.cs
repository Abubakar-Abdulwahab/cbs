using Orchard;
using Parkway.CBS.Client.Web.ViewModels;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface ITCCRequestHistoryHandler : IDependency
    {
        /// <summary>
        /// Gets TCC Requests
        /// </summary>
        /// <param name="searchParams">search params</param>
        /// <returns>TCCRequestHistoryVM</returns>
        TCCRequestHistoryVM GetRequests(TCCReportSearchParams searchParams);


        /// <summary>
        /// Get Paged TCC Requests
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="page">page</param>
        /// <returns>APIResponse</returns>
        APIResponse GetPagedRequestsData(string token, int? page);
    }
}
