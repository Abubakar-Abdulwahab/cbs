using NHibernate;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Police.Core.DataFilters.PoliceOfficerReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.PoliceOfficerReport.SearchFilters;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Police.Core.Models;
using NHibernate.Criterion;
using NHibernate.Transform;
using Parkway.CBS.Core.HelperModels;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.PoliceOfficerReport
{
    public class PoliceOfficerFilter : IPoliceOfficerFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<IPoliceOfficerReportFilters> _searchFilters;

        public PoliceOfficerFilter(IOrchardServices orchardService, IEnumerable<IPoliceOfficerReportFilters> searchFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchFilters = searchFilters;
        }

        /// <summary>
        /// Get veiw model for request reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfActiveOfficers }</returns>
        public dynamic GetRequestReportViewModel(PoliceOfficerSearchParams searchParams, bool applyAccessRestrictions)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams, applyAccessRestrictions);
                returnOBJ.TotalNumberOfActiveOfficers = GetTotalNumberOfActiveOfficers(searchParams, applyAccessRestrictions);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the total number of active officers
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfActiveOfficers(PoliceOfficerSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions);

            return query
                .Add(Restrictions.Eq("IsActive", true))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.CountDistinct<PoliceOfficer>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the list of active police officers
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="applyAccessRestrictions"></param>
        /// <returns>IEnumerable{PoliceOfficerVM}</returns>
        private IEnumerable<PoliceOfficerVM> GetReport(PoliceOfficerSearchParams searchParams, bool applyAccessRestrictions)
        {
            var query = GetCriteria(searchParams, applyAccessRestrictions, false);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .Add(Restrictions.Eq("IsActive", true))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property("Id"), nameof(PoliceOfficerVM.Id))
                .Add(Projections.Property("Name"), nameof(PoliceOfficerVM.Name))
                .Add(Projections.Property("IdentificationNumber"), nameof(PoliceOfficerVM.IdNumber))
                .Add(Projections.Property("IPPISNumber"), nameof(PoliceOfficerVM.IppisNumber))
                .Add(Projections.Property("IsActive"), nameof(PoliceOfficerVM.IsActive))
                .Add(Projections.Property("Command.Id"), nameof(PoliceOfficerVM.CommandId))
                .Add(Projections.Property("Command.Name"), nameof(PoliceOfficerVM.CommandName))
                .Add(Projections.Property("Command.Code"), nameof(PoliceOfficerVM.CommandCode))
                .Add(Projections.Property("OfficerRank.Id"), nameof(PoliceOfficerVM.RankId))
                .Add(Projections.Property("OfficerRank.RankName"), nameof(PoliceOfficerVM.RankName)))
                .AddOrder(Order.Desc("Id"))
                .SetResultTransformer(Transformers.AliasToBean<PoliceOfficerVM>())
                .Future<PoliceOfficerVM>();
        }


        public ICriteria GetCriteria(PoliceOfficerSearchParams searchParams, bool applyAccessRestrictions, bool addAlias = true)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<PoliceOfficer>(nameof(PoliceOfficer));

            criteria.CreateAlias(nameof(PoliceOfficer.Command), "Command")
                .CreateAlias(nameof(PoliceOfficer.Rank), "OfficerRank");
                

            foreach (var filter in _searchFilters)
            {
                filter.AddCriteriaRestriction(criteria, searchParams, addAlias);
            }

            return criteria;
        }
    }
}