using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.OfficerDeploymentAllowance.Contracts;
using Parkway.CBS.Police.Core.DataFilters.OfficerDeploymentAllowance.SearchFilters;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.OfficerDeploymentAllowance
{
    public class OfficerDeploymentAllowanceFilter : IOfficerDeploymentAllowanceFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<IOfficerDeploymentAllowanceFilters> _searchFilters;

        public OfficerDeploymentAllowanceFilter(IOrchardServices orchardService, IEnumerable<IOfficerDeploymentAllowanceFilters> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        public dynamic GetRequestReportViewModel(OfficerDeploymentAllowanceSearchParams searchParams, bool applyAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();
            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams, applyAccessRestrictions);
                returnOBJ.TotalRecordCount = GetAggregateTotalRecordCount(searchParams, applyAccessRestrictions);
                returnOBJ.TotalAllowanceAmount = GetAggregateInvoiceAmount(searchParams, applyAccessRestrictions);
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
        /// Get the aggregate of the requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetAggregateInvoiceAmount(OfficerDeploymentAllowanceSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Sum<PoliceofficerDeploymentAllowance>(x => x.Amount), "TotalAmount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }
        
        /// <summary>
        /// Get the aggregate of the requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetAggregateTotalRecordCount(OfficerDeploymentAllowanceSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            return query.SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<PoliceofficerDeploymentAllowance>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }
        
        /// <summary>
        /// Get the list of requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PSSRequestVM}</returns>
        private IEnumerable<PoliceOfficerDeploymentAllowanceVM> GetReport(OfficerDeploymentAllowanceSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
            .AddOrder(Order.Desc($"{nameof(PoliceofficerDeploymentAllowance.Id)}"))
           .SetProjection(Projections.ProjectionList()
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.Name)}"), nameof(PoliceOfficerDeploymentAllowanceVM.PoliceOfficerName))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.AccountNumber)}"), nameof(PoliceOfficerDeploymentAllowanceVM.AccountNumber))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.BankCode)}"), nameof(PoliceOfficerDeploymentAllowanceVM.BankCode))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.Id)}"), nameof(PoliceOfficerDeploymentAllowanceVM.PolicerOfficerId))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.Status)}"), nameof(PoliceOfficerDeploymentAllowanceVM.StatusId))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.Comment)}"), nameof(PoliceOfficerDeploymentAllowanceVM.Comment))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.IPPISNumber)}"), nameof(PoliceOfficerDeploymentAllowanceVM.PoliceOfficerIPPIS))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.Id)}"), nameof(PoliceOfficerDeploymentAllowanceVM.Id))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.Amount)}"), nameof(PoliceOfficerDeploymentAllowanceVM.Amount))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.Rank)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.Rank.RankName)}"), nameof(PoliceOfficerDeploymentAllowanceVM.PoliceOfficerRank))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.IdentificationNumber)}"), nameof(PoliceOfficerDeploymentAllowanceVM.PoliceOfficerAPNumber))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.Request)}.{nameof(PoliceofficerDeploymentAllowance.Request.FileRefNumber)}"), nameof(PoliceOfficerDeploymentAllowanceVM.FileRefNumber))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.Request)}.{nameof(PoliceofficerDeploymentAllowance.Request.Id)}"), nameof(PoliceOfficerDeploymentAllowanceVM.RequestId))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.Invoice)}.{nameof(PoliceofficerDeploymentAllowance.Invoice.InvoiceNumber)}"), nameof(PoliceOfficerDeploymentAllowanceVM.InvoiceNumber))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.InitiatedBy)}"), nameof(PoliceOfficerDeploymentAllowanceVM.InitiatedBy))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.CreatedAtUtc)}"), nameof(PoliceOfficerDeploymentAllowanceVM.RequestDate))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.UpdatedAtUtc)}"), nameof(PoliceOfficerDeploymentAllowanceVM.LastActionDate))
           .Add(Projections.Property($"{nameof(PoliceofficerDeploymentAllowance.Narration)}"), nameof(PoliceOfficerDeploymentAllowanceVM.Narration))
           ).SetResultTransformer(Transformers.AliasToBean<PoliceOfficerDeploymentAllowanceVM>()).Future<PoliceOfficerDeploymentAllowanceVM>();
        }
        
        public ICriteria GetCriteria(OfficerDeploymentAllowanceSearchParams searchParams, bool applyAccessRestrictions)
        {
            ICriteria criteria = _transactionManager.GetSession().CreateCriteria<PoliceofficerDeploymentAllowance>(nameof(PoliceofficerDeploymentAllowance));
            criteria
               .CreateAlias($"{nameof(PoliceofficerDeploymentAllowance)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}", $"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}")
               .CreateAlias($"{nameof(PoliceofficerDeploymentAllowance)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog)}.{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.Rank)}", $"{nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog.Rank)}")
               .CreateAlias($"{nameof(PoliceofficerDeploymentAllowance)}.{nameof(PoliceofficerDeploymentAllowance.Request)}", $"{nameof(PoliceofficerDeploymentAllowance.Request)}")
               .CreateAlias($"{nameof(PoliceofficerDeploymentAllowance)}.{nameof(PoliceofficerDeploymentAllowance.Invoice)}", $"{nameof(PoliceofficerDeploymentAllowance.Invoice)}")
;
            criteria.Add(Restrictions.Between(nameof(PoliceofficerDeploymentAllowance.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams);
            }

            if (applyAccessRestrictions)
            {
                var commandCriteria = DetachedCriteria.For<Command>("cm")
                    .Add(Restrictions.EqProperty("Id", "PoliceofficerDeploymentAllowance.Command.Id"))
                    .SetProjection(Projections.Constant(1));

                var accessListCriteria = DetachedCriteria.For<ApprovalAccessList>("aal")
                .Add(Restrictions.Disjunction()
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.IsNull("State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.IsNull("LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.IsNull("Command.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.IsNull("Service.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Service.Id", "Request.Service.Id")), Restrictions.IsNull("Command.Id")))
                .Add(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.And(Restrictions.EqProperty("CommandCategory.Id", "cm.CommandCategory.Id"), Restrictions.EqProperty("State.Id", "cm.State.Id")), Restrictions.EqProperty("LGA.Id", "cm.LGA.Id")), Restrictions.EqProperty("Command.Id", "cm.Id")), Restrictions.EqProperty("Service.Id", "Request.Service.Id"))))
                .Add(Restrictions.And(Restrictions.Eq("aal.ApprovalAccessRoleUser.Id", searchParams.ApprovalAccessRoleUserId),Restrictions.Eq("aal.IsDeleted", false)))
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

    }
}