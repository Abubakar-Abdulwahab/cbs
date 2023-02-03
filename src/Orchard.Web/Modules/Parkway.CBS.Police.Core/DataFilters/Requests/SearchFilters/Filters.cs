using NHibernate;
using NHibernate.Criterion;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters
{

    /// <summary>
    /// filter for file number
    /// </summary>
    public class FileNumberFilter : IPoliceRequestSearchFilter
    {
        private bool DoCheck(string fileNumber)
        {
            return !string.IsNullOrEmpty(fileNumber);
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.FileNumber))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.FileRefNumber == searchParams.RequestOptions.FileNumber));
            }
        }

        public void AddCriteriaRestrictionForInvoiceNumber(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.FileNumber))
            {
                criteria.Add(Restrictions.Where<PSSRequestInvoice>(x => x.Request.FileRefNumber == searchParams.RequestOptions.FileNumber));
            }
        }

        public void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.FileNumber))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.FileRefNumber == searchParams.RequestOptions.FileNumber));
            }
        }
    }


    public class ServiceTypeFilter : IPoliceRequestSearchFilter
    {
        private bool DoCheck(int intValueSelectedServiceId)
        {
            return intValueSelectedServiceId != 0;
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.IntValueSelectedServiceId))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.Service == new PSService { Id = searchParams.IntValueSelectedServiceId }));
            }
        }

        public void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.IntValueSelectedServiceId))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.Service == new PSService { Id = searchParams.IntValueSelectedServiceId }));
            }
        }

        public void AddCriteriaRestrictionForInvoiceNumber(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.IntValueSelectedServiceId))
            {
                criteria.Add(Restrictions.Where<PSSRequestInvoice>(x => x.Request.Service == new PSService { Id = searchParams.IntValueSelectedServiceId }));
            }
        }
    }


    public class RequestStatusFilter : IPoliceRequestSearchFilter
    {
        private bool DoCheck(Models.Enums.PSSRequestStatus requestStatus)
        {
            return requestStatus != Models.Enums.PSSRequestStatus.None;
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.RequestStatus))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.Status == (int)searchParams.RequestOptions.RequestStatus));
            }
        }

        public void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.RequestStatus))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.Status == (int)searchParams.RequestOptions.RequestStatus));
            }
        }

        public void AddCriteriaRestrictionForInvoiceNumber(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.RequestStatus))
            {
                criteria.Add(Restrictions.Where<PSSRequestInvoice>(x => x.Request.Status == (int)searchParams.RequestOptions.RequestStatus));
            }
        }
    }


    public class TaxEntityFilter : IPoliceRequestSearchFilter
    {
        private bool DoCheck(long taxEntityId)
        {
            return taxEntityId != 0;
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.TaxEntityId))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.TaxEntity == new TaxEntity { Id = searchParams.TaxEntityId }));
            }
        }

        public void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.TaxEntityId))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.TaxEntity == new TaxEntity { Id = searchParams.TaxEntityId }));
            }
        }

        public void AddCriteriaRestrictionForInvoiceNumber(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.TaxEntityId))
            {
                criteria.Add(Restrictions.Where<PSSRequestInvoice>(x => x.Request.TaxEntity == new TaxEntity { Id = searchParams.TaxEntityId }));
            }
        }
    }


    /// <summary>
    /// filter for approval number
    /// </summary>
    public class ApprovalNumberFilter : IPoliceRequestSearchFilter
    {
        private bool DoCheck(string approvalNumber)
        {
            return !string.IsNullOrEmpty(approvalNumber);
        }


        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.ApprovalNumber))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.ApprovalNumber == searchParams.RequestOptions.ApprovalNumber));
            }
        }

        public void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.ApprovalNumber))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.ApprovalNumber == searchParams.RequestOptions.ApprovalNumber));
            }
        }


        public void AddCriteriaRestrictionForInvoiceNumber(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.RequestOptions.ApprovalNumber))
            {
                criteria.Add(Restrictions.Where<PSSRequestInvoice>(x => x.Request.ApprovalNumber == searchParams.RequestOptions.ApprovalNumber));
            }
        }
    }

    /// <summary>
    /// Filter for Command
    /// </summary>
    public class CommandFilter : IPoliceRequestSearchFilter
    {
        private bool DoCheck(int commandId)
        {
            return commandId != 0;
        }

        private bool DoCheckForAllCommands(int commandId, int lgaId)
        {
            return commandId == 0 && lgaId != 0;
        }

        private bool DoCheckForNoCommands(int commandId)
        {
            return commandId == -1;
        }

        public void AddCriteriaRestriction(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if(DoCheckForAllCommands(searchParams.CommandId, searchParams.LGA))
            {
                var allCommandsCriteria = DetachedCriteria.For<Command>("Cmd")
                    .Add(Restrictions.And(Restrictions.EqProperty("Id", "pr.Command.Id"), Restrictions.Eq("LGA.Id", searchParams.LGA)))
                    .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(allCommandsCriteria));

            }
            else if (DoCheck(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.Command == new Command { Id = searchParams.CommandId }));
            }
            else if (DoCheckForNoCommands(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.Command == new Command { Id = 0 }));
            }
        }

        public void AddDetachedCriteriaRestriction(DetachedCriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheckForAllCommands(searchParams.CommandId, searchParams.LGA))
            {
                var allCommandsCriteria = DetachedCriteria.For<Command>("Cmd")
                    .Add(Restrictions.And(Restrictions.EqProperty("Id", "pr.Command.Id"), Restrictions.Eq("LGA.Id", searchParams.LGA)))
                    .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(allCommandsCriteria));
            }
            else if (DoCheck(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.Command == new Command { Id = searchParams.CommandId }));
            }
            else if (DoCheckForNoCommands(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.Command == new Command { Id = 0 }));
            }
        }

        public void AddCriteriaRestrictionForInvoiceNumber(ICriteria criteria, RequestsReportSearchParams searchParams)
        {
            if (DoCheck(searchParams.CommandId))
            {
                criteria.Add(Restrictions.Where<PSSRequestInvoice>(x => x.Request.Command == new Command { Id = searchParams.CommandId }));
            }
        }
    }


}