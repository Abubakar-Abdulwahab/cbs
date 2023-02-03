using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.OfficerDeployment.Contracts;
using Parkway.CBS.Police.Core.DataFilters.OfficerDeployment.SearchFilters;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;


namespace Parkway.CBS.Police.Core.DataFilters.OfficerDeployment
{
    public class OfficerDeploymentFilter : IOfficerDeploymentFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<IOfficerDeploymentReportFilters> _searchFilters;

        public OfficerDeploymentFilter(IOrchardServices orchardService, IEnumerable<IOfficerDeploymentReportFilters> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }


        /// <summary>
        /// Get veiw model for request reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, Aggregate }</returns>
        public dynamic GetRequestReportViewModel(OfficerDeploymentSearchParams searchParams, bool applyAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams, applyAccessRestrictions);
                returnOBJ.TotalNumberOfDeployments = GetTotalNumberOfDeployments(searchParams, applyAccessRestrictions);
                returnOBJ.TotalNumberOfActiveDeployments = GetTotalNumberOfActiveDeployments(searchParams, applyAccessRestrictions);
                returnOBJ.TotalNumberOfOfficersInActiveDeployments = GetTotalNumberOfOfficersInActiveDeployments(searchParams, applyAccessRestrictions);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the aggregate of the requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfOfficersInActiveDeployments(OfficerDeploymentSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            return query
                .Add(Restrictions.Eq("IsActive", true))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PoliceOfficerDeploymentLog>(x => x.PoliceOfficerLog.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the aggregate of the total number of active deployments
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfActiveDeployments(OfficerDeploymentSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            return query
                .Add(Restrictions.Eq("IsActive", true))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PoliceOfficerDeploymentLog>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the aggregate of the total number of deployments
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfDeployments(OfficerDeploymentSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PoliceOfficerDeploymentLog>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the list of requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PSSRequestVM}</returns>
        private IEnumerable<PoliceOfficerDeploymentVM> GetReport(OfficerDeploymentSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions, false);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return  query
                .AddOrder(Order.Desc("Id"))
                .CreateAlias($"{nameof(PSSRequest)}.{nameof(PSSRequest.CBSUser)}", nameof(CBS.Core.Models.CBSUser))
                .CreateAlias(nameof(PoliceOfficerDeploymentLog.OfficerRank), nameof(PoliceRanking))
                .CreateAlias(nameof(PoliceOfficerDeploymentLog.Command), nameof(Command))
                .CreateAlias(nameof(PoliceOfficerDeploymentLog.State), nameof(CBS.Core.Models.StateModel))
                .CreateAlias(nameof(PoliceOfficerDeploymentLog.LGA), nameof(CBS.Core.Models.LGA))
                .CreateAlias(nameof(PoliceOfficerDeploymentLog.Invoice), nameof(CBS.Core.Models.Invoice))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property($"{nameof(PSSRequest)}.{nameof(PSSRequest.FileRefNumber)}"), nameof(PoliceOfficerDeploymentVM.FileRefNumber))
                .Add(Projections.Property($"{nameof(PSSRequest)}.{nameof(PSSRequest.ApprovalNumber)}"), nameof(PoliceOfficerDeploymentVM.ApprovalNumber))
                .Add(Projections.Property(nameof(PoliceOfficerDeploymentLog.Address)), nameof(PoliceOfficerDeploymentVM.Address))
                .Add(Projections.Property($"{nameof(CBS.Core.Models.CBSUser)}.{nameof(CBS.Core.Models.CBSUser.Name)}"), nameof(PoliceOfficerDeploymentVM.CustomerName))
                .Add(Projections.Property($"{nameof(PSSRequest)}.{nameof(PSSRequest.Id)}"), nameof(PoliceOfficerDeploymentVM.RequestId))
                .Add(Projections.Property($"{nameof(PolicerOfficerLog)}.{nameof(PolicerOfficerLog.Id)}"), nameof(PoliceOfficerDeploymentVM.PolicerOfficerId))
                .Add(Projections.Property($"{nameof(PolicerOfficerLog)}.{nameof(PolicerOfficerLog.IPPISNumber)}"), nameof(PoliceOfficerDeploymentVM.PoliceOfficerIPPIS))
                .Add(Projections.Property($"{nameof(PolicerOfficerLog)}.{nameof(PolicerOfficerLog.IdentificationNumber)}"), nameof(PoliceOfficerDeploymentVM.PoliceOfficerIdentificationNumber))
                .Add(Projections.Property(nameof(PoliceOfficerDeploymentLog.OfficerName)), nameof(PoliceOfficerDeploymentVM.PoliceOfficerName))
                .Add(Projections.Property($"{nameof(PoliceRanking)}.{nameof(PoliceRanking.RankName)}"), nameof(PoliceOfficerDeploymentVM.PoliceOfficerRank))
                .Add(Projections.Property(nameof(PoliceOfficerDeploymentLog.IsActive)), nameof(PoliceOfficerDeploymentVM.IsActive))
                .Add(Projections.Property(nameof(PoliceOfficerDeploymentLog.Status)), nameof(PoliceOfficerDeploymentVM.Status))
                .Add(Projections.Property($"{nameof(CBS.Core.Models.StateModel)}.{nameof(CBS.Core.Models.StateModel.Name)}"), nameof(PoliceOfficerDeploymentVM.StateName))
                .Add(Projections.Property($"{nameof(CBS.Core.Models.LGA)}.{nameof(CBS.Core.Models.LGA.Name)}"), nameof(PoliceOfficerDeploymentVM.LGAName))
                .Add(Projections.Property($"{nameof(CBS.Core.Models.Invoice)}.{nameof(CBS.Core.Models.Invoice.InvoiceNumber)}"), nameof(PoliceOfficerDeploymentVM.InvoiceNumber))
                .Add(Projections.Property(nameof(PoliceOfficerDeploymentLog.StartDate)), nameof(PoliceOfficerDeploymentVM.StartDate))
                .Add(Projections.Property(nameof(PoliceOfficerDeploymentLog.EndDate)), nameof(PoliceOfficerDeploymentVM.EndDate))
                .Add(Projections.Property($"{nameof(Command)}.{nameof(Command.Name)}"), nameof(PoliceOfficerDeploymentVM.CommandName))
                .Add(Projections.Property(nameof(PoliceOfficerDeploymentLog.Id)), nameof(PoliceOfficerDeploymentVM.Id))
                ).SetResultTransformer(Transformers.AliasToBean<PoliceOfficerDeploymentVM>())
                .Future<PoliceOfficerDeploymentVM>();
        }


        public ICriteria GetCriteria(OfficerDeploymentSearchParams searchParams, bool applyAccessRestrictions, bool addAlias = true)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PoliceOfficerDeploymentLog>(nameof(PoliceOfficerDeploymentLog))
                .CreateAlias(nameof(PoliceOfficerDeploymentLog.PoliceOfficerLog), nameof(PolicerOfficerLog))
                .CreateAlias(nameof(PoliceOfficerDeploymentLog.Request), nameof(PSSRequest))
                .CreateAlias($"{nameof(PolicerOfficerLog)}.{nameof(PolicerOfficerLog.Command)}", "polcmd");

            criteria
                .Add(Restrictions.Between(nameof(PoliceOfficerDeploymentLog.StartDate), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams, addAlias);
            }

            if (applyAccessRestrictions)
            {
                criteria.CreateAlias($"{nameof(PSSRequest)}.{nameof(PSSRequest.Service)}", nameof(PSService));

                var commandCriteria = DetachedCriteria.For<Command>("cm")
                    .Add(Restrictions.EqProperty("Id", "polcmd.Id"))
                    .SetProjection(Projections.Constant(1));

                var accessListCriteria = DetachedCriteria.For<ApprovalAccessList>("aal")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.IsNull("State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id"))))
                .Add(Restrictions.EqProperty($"{nameof(PSService)}.{nameof(PSService.Id)}", "Service.Id"))
                .Add(Restrictions.And(Restrictions.Eq("ApprovalAccessRoleUser.Id", searchParams.ApprovalAccessRoleUserId), Restrictions.Eq("IsDeleted", false)))
                .SetProjection(Projections.Constant(1));

                commandCriteria.Add(Subqueries.Exists(accessListCriteria));
                criteria.Add(Subqueries.Exists(commandCriteria));
            }

            return criteria;
        }
    }
}