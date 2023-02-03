using System;
using Orchard;
using NHibernate;
using Orchard.Data;
using System.Dynamic;
using NHibernate.Criterion;
using NHibernate.Transform;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.DataFilters.Requests.Contracts;
using Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.DataFilters.Requests
{
    public class PoliceRequestFilter : IPoliceRequestFilter
    {
        private readonly IEnumerable<Lazy<IPoliceRequestSearchFilter>> _searchFilters;
        private readonly IEnumerable<Lazy<IPoliceCoporateBranchRequestSearchFilter>> _searchBranchFilters;
        private readonly ITransactionManager _transactionManager;
        public PoliceRequestFilter(IOrchardServices orchardService, IEnumerable<Lazy<IPoliceRequestSearchFilter>> searchFilters, IEnumerable<Lazy<IPoliceCoporateBranchRequestSearchFilter>> searchBranchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
            _searchBranchFilters = searchBranchFilters;
        }


        public ICriteria GetCriteria(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            ICriteria criteria = _transactionManager.GetSession().CreateCriteria<PSSRequest>("pr");
            criteria.Add(Restrictions.Between(nameof(PSSRequest.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            if (applyApprovalAccessRestrictions)
            {
                var requestFlowApproverCriteria = DetachedCriteria.For<PSServiceRequestFlowApprover>("PRF")
            .Add(Restrictions.Eq("AssignedApprover.Id", searchParams.AdminUserId))
            .Add(Restrictions.EqProperty("FlowDefinitionLevel.Id", "pr.FlowDefinitionLevel.Id"))
            .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(requestFlowApproverCriteria));
            }

            foreach (var filter in _searchFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            if (applyAccessRestrictions)
            {
                var commandCriteria = DetachedCriteria.For<Command>("cm")
                    .Add(Restrictions.EqProperty("Id", "pr.Command.Id"))
                    .SetProjection(Projections.Constant(1));

                var accessListCriteria = DetachedCriteria.For<ApprovalAccessList>("aal")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.IsNull("State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Service.Id", "pr.Service.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.EqProperty("Service.Id", "pr.Service.Id"))))
                .SetProjection(Projections.Constant(1));

                var accessRoleUserCriteria = DetachedCriteria.For<ApprovalAccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", searchParams.AdminUserId))
                    .Add(Restrictions.EqProperty("Id", "aal.ApprovalAccessRoleUser.Id"))
                    .SetProjection(Projections.Constant(1));


                accessListCriteria.Add(Subqueries.Exists(accessRoleUserCriteria));
                commandCriteria.Add(Subqueries.Exists(accessListCriteria));
                criteria.Add(Subqueries.Exists(commandCriteria));
            }

            return criteria;
        }

        public DetachedCriteria GetDetachedCriteria(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            DetachedCriteria detachedCriteria = DetachedCriteria.For<PSSRequest>("pr");
            detachedCriteria.Add(Restrictions.Between(nameof(PSSRequest.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));


            if (applyApprovalAccessRestrictions)
            {
                var requestFlowApproverCriteria = DetachedCriteria.For<PSServiceRequestFlowApprover>("PRF")
            .Add(Restrictions.Eq("AssignedApprover.Id", searchParams.AdminUserId))
            .Add(Restrictions.EqProperty("FlowDefinitionLevel.Id", "pr.FlowDefinitionLevel.Id"))
            .SetProjection(Projections.Constant(1));
                detachedCriteria.Add(Subqueries.Exists(requestFlowApproverCriteria));
            }

            foreach (var filter in _searchFilters)
            {
                filter.Value.AddDetachedCriteriaRestriction(detachedCriteria, searchParams);
            }

            if (applyAccessRestrictions)
            {
                var commandCriteria = DetachedCriteria.For<Command>("cm")
                    .Add(Restrictions.EqProperty("Id", "pr.Command.Id"))
                    .SetProjection(Projections.Constant(1));

                var accessListCriteria = DetachedCriteria.For<ApprovalAccessList>("aal")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.IsNull("State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Service.Id", "pr.Service.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.EqProperty("Service.Id", "pr.Service.Id"))))
                .SetProjection(Projections.Constant(1));

                var accessRoleUserCriteria = DetachedCriteria.For<ApprovalAccessRoleUser>("aru")
                    .Add(Restrictions.Eq("User.Id", searchParams.AdminUserId))
                    .Add(Restrictions.EqProperty("Id", "aal.ApprovalAccessRoleUser.Id"))
                    .SetProjection(Projections.Constant(1));

                accessListCriteria.Add(Subqueries.Exists(accessRoleUserCriteria));
                commandCriteria.Add(Subqueries.Exists(accessListCriteria));
                detachedCriteria.Add(Subqueries.Exists(commandCriteria));
            }

            return detachedCriteria;
        }

        /// <summary>
        /// Get veiw model for request reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetRequestReportViewModel(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();
            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
                returnOBJ.TotalRecordCount = GetAggregateTotalRecordCount(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
                returnOBJ.TotalInvoiceCount = GetAggregateInvoiceCount(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
                returnOBJ.TotalInvoiceAmount = GetAggregateInvoiceAmount(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<PSSRequestVM>();
                returnOBJ.TotalRecordCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
                returnOBJ.TotalInvoiceCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
                returnOBJ.TotalInvoiceAmount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
            }
            return returnOBJ;
        }

        /// <summary>
        /// Get veiw model for request reports for sub users
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetRequestBranchReportViewModel(RequestsReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();
            try
            {
                returnOBJ.ReportRecords = GetBranchReport(searchParams);
                returnOBJ.TotalRecordCount = GetBranchAggregateTotalRecordCount(searchParams);
            }
            catch (NoRecordFoundException)
            {
                returnOBJ.ReportRecords = new List<PSSRequestVM>();
                returnOBJ.TotalRecordCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
                returnOBJ.TotalInvoiceCount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
                returnOBJ.TotalInvoiceAmount = new List<ReportStatsVM> { new ReportStatsVM { TotalRecordCount = 0 } };
            }
            return returnOBJ;
        }
        
        public ICriteria GetBranchCriteria(RequestsReportSearchParams searchParams)
        {
            ICriteria criteria = _transactionManager.GetSession().CreateCriteria<PSSRequest>("pr")
           .CreateAlias(nameof(PSSRequest.TaxEntity), "TaxEntity")
           .CreateAlias(nameof(PSSRequest.CBSUser), nameof(PSSRequest.CBSUser))
           .CreateAlias(nameof(PSSRequest.TaxEntityProfileLocation), "Location")
           .Add(Restrictions.Between(nameof(PSSRequest.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            if (searchParams.RequestOptions.RequestStatus != Models.Enums.PSSRequestStatus.None)
            {
                criteria.Add(Restrictions.Where<PSSRequest>(x => x.Status == (int)searchParams.RequestOptions.RequestStatus));
            }

            var requestCriteria = DetachedCriteria.For<CBSUserTaxEntityProfileLocation>("CBSUTPL")
           .CreateAlias("CBSUTPL.CBSUser", "CBSUsr")
           .CreateAlias("CBSUsr.TaxEntity", "TE")
           .CreateAlias("CBSUTPL.TaxEntityProfileLocation", "TaxEntityProfileLocation")
           .Add(Restrictions.EqProperty("TE.Id", "TaxEntity.Id"));
           

            if (searchParams.IsBranchAdmin)
            {
                requestCriteria.Add(Restrictions.Where<CBSUserTaxEntityProfileLocation>(x => x.TaxEntityProfileLocation.TaxEntity == new TaxEntity { Id = searchParams.TaxEntityId }));
            }
            else
            {
                criteria.Add(Restrictions.Eq($"{nameof(PSSRequest.CBSUser)}.{nameof(PSSRequest.CBSUser.Id)}", searchParams.CBSUserId));
            }

            foreach (var filter in _searchBranchFilters)
            {
                filter.Value.AddDetachedCriteriaRestriction(requestCriteria, searchParams);
            }

            criteria.Add(Subqueries.PropertyIn($"{nameof(PSSRequest.CBSUser)}.{nameof(CBSUser.Id)}", requestCriteria.SetProjection(Projections.Property($"CBSUsr.{nameof(CBSUser.Id)}"))));

            return criteria;
        }

        /// <summary>
        /// Get the aggregate of the requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetAggregateInvoiceAmount(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            var detachedQuery = GetDetachedCriteria(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
            detachedQuery.SetProjection(Projections.Property<PSSRequest>(x => x.Id));

            var query = _transactionManager.GetSession()
                .CreateCriteria<PSSRequestInvoice>("PSRI")
                .CreateAlias(nameof(PSSRequestInvoice.Invoice), "Invoice")
                .Add(Restrictions.Between(nameof(PSSRequestInvoice.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate))
                .Add(Restrictions.Where<PSSRequestInvoice>(x => x.Invoice.Status != (int)InvoiceStatus.WriteOff))
            .SetProjection(Projections.Property("Request"));

            query.Add(Subqueries.PropertyIn("Request.Id", detachedQuery));

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Sum<PSSRequestInvoice>(x => x.Invoice.Amount), "TotalAmount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get the aggregate of the requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetAggregateInvoiceCount(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            var query = GetDetachedCriteria(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
            query.SetProjection(Projections.Property<PSSRequest>(x => x.Id));

            var subquery = _transactionManager.GetSession().CreateCriteria<PSSRequestInvoice>("PSRI")
                .Add(Restrictions.Between(nameof(PSSRequestInvoice.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate))
            .SetProjection(Projections.Property("Request"));

            subquery.Add(Subqueries.PropertyIn("Request.Id", query));

            return subquery.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSRequestInvoice>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();

        }

        /// <summary>
        /// Get the aggregate of the requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetAggregateTotalRecordCount(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSRequest>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get the list of requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PSSRequestVM}</returns>
        private IEnumerable<PSSRequestVM> GetReport(RequestsReportSearchParams searchParams, bool applyAccessRestrictions, bool applyApprovalAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions, applyApprovalAccessRestrictions);
            string orderByColumnName = (string.IsNullOrEmpty(searchParams.OrderByColumnName) ? "Id" : searchParams.OrderByColumnName);
            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
            .AddOrder(Order.Desc(orderByColumnName))
           .CreateAlias(nameof(PSSRequest.Service), "Service")
           .CreateAlias(nameof(PSSRequest.TaxEntity), "TaxEntity")
           .SetProjection(Projections.ProjectionList()
           .Add(Projections.Property("FileRefNumber"), nameof(PSSRequestVM.FileRefNumber))
           .Add(Projections.Property("ApprovalNumber"), nameof(PSSRequestVM.ApprovalNumber))
           .Add(Projections.Property("Status"), nameof(PSSRequestVM.StatusId))
           .Add(Projections.Property("TaxEntity.Recipient"), nameof(PSSRequestVM.CustomerName))
           .Add(Projections.Property("Id"), nameof(PSSRequestVM.Id))
           .Add(Projections.Property("Service.ServiceType"), nameof(PSSRequestVM.ServiceTypeId))
           .Add(Projections.Property("Service.Name"), nameof(PSSRequestVM.ServiceName))
           .Add(Projections.Property("CreatedAtUtc"), nameof(PSSRequestVM.RequestDate))
           .Add(Projections.Property("UpdatedAtUtc"), nameof(PSSRequestVM.LastActionDate))
           ).SetResultTransformer(Transformers.AliasToBean<PSSRequestVM>()).Future<PSSRequestVM>();
        }

        /// <summary>
        /// Get the aggregate of the requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetBranchAggregateTotalRecordCount(RequestsReportSearchParams searchParams)
        {
            var query = GetBranchCriteria(searchParams);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PSSRequest>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        private IEnumerable<PSSRequestVM> GetBranchReport(RequestsReportSearchParams searchParams)
        {
            var query = GetBranchCriteria(searchParams);


            string orderByColumnName = (string.IsNullOrEmpty(searchParams.OrderByColumnName) ? "Id" : searchParams.OrderByColumnName);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
          .AddOrder(Order.Desc(orderByColumnName))
         .CreateAlias(nameof(PSSRequest.Service), "Service")
         .SetProjection(Projections.ProjectionList()
         .Add(Projections.Property("FileRefNumber"), nameof(PSSRequestVM.FileRefNumber))
         .Add(Projections.Property("ApprovalNumber"), nameof(PSSRequestVM.ApprovalNumber))
         .Add(Projections.Property("Status"), nameof(PSSRequestVM.StatusId))
         .Add(Projections.Property($"Location.{nameof(TaxEntityProfileLocation.Name)}"), nameof(PSSRequestVM.BranchName))
         .Add(Projections.Property($"{nameof(PSSRequest.CBSUser)}.{nameof(PSSRequest.CBSUser.Name)}"), nameof(PSSRequestVM.CustomerName))
         .Add(Projections.Property("Id"), nameof(PSSRequestVM.Id))
         .Add(Projections.Property("Service.ServiceType"), nameof(PSSRequestVM.ServiceTypeId))
         .Add(Projections.Property("Service.Name"), nameof(PSSRequestVM.ServiceName))
         .Add(Projections.Property("CreatedAtUtc"), nameof(PSSRequestVM.RequestDate))
         .Add(Projections.Property("UpdatedAtUtc"), nameof(PSSRequestVM.LastActionDate))
         ).SetResultTransformer(Transformers.AliasToBean<PSSRequestVM>()).Future<PSSRequestVM>();
        }
    }
}