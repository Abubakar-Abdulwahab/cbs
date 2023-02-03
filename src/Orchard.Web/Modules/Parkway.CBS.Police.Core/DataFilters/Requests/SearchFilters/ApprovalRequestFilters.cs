using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters
{

    /// <summary>
    /// filter for file number
    /// </summary>
    public class FileNumberApprovalFilter : IAdminApprovalRequestSearchFilter
    {
        private bool DoCheck(string fileNumber)
        {
            return !string.IsNullOrEmpty(fileNumber);
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.FileNumber))
            {
                criteria.Add(Restrictions.Where<RequestCommandWorkFlowLog>(x => x.Request.FileRefNumber == searchParams.RequestOptions.FileNumber));
            }
        }
    }


    public class AdminApprovalServiceTypeFilter : IAdminApprovalRequestSearchFilter
    {
        private bool DoCheck(int intValueSelectedServiceId)
        {
            return intValueSelectedServiceId != 0;
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.IntValueSelectedServiceId))
            {
                criteria.Add(Restrictions.Where<RequestCommandWorkFlowLog>(x => x.Request.Service == new PSService { Id = searchParams.IntValueSelectedServiceId }));
            }
        }
    }

    public class AdminRequestStatusFilter : IAdminApprovalRequestSearchFilter
    {
        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (searchParams.RequestOptions.RequestStatus != Models.Enums.PSSRequestStatus.None)
            {
                criteria.Add(Restrictions.Eq(nameof(RequestCommandWorkFlowLog.Request) + "." + nameof(RequestCommandWorkFlowLog.Request.Status), (int)searchParams.RequestOptions.RequestStatus));
            }
        }
    }

}