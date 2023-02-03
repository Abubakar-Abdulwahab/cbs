using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.CommandReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.CommandReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.CommandReport
{
    public class CommandWalletReportFilter : ICommandWalletReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<Lazy<ICommandWalletReportFilters>> _searchCommandReportFilters;

        public CommandWalletReportFilter(IOrchardServices orchardService, IEnumerable<Lazy<ICommandWalletReportFilters>> searchCommandReportFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchCommandReportFilters = searchCommandReportFilters;
        }

        /// <summary>
        /// Get veiw model for commands reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfActiveOfficers }</returns>
        public dynamic GetCommandWalletReportViewModel(CommandWalletReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfActiveCommands = GetTotalNumberOfActiveCommandsWallet(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the total number of active commands
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberOfActiveCommandsWallet(CommandWalletReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .Add(Restrictions.Eq("IsActive", true))
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.CountDistinct<CommandWalletDetails>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the list of active commands
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns><see cref="IEnumerable{Command}"/></returns>
        private IEnumerable<CommandWalletReportVM> GetReport(CommandWalletReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .Add(Restrictions.Eq("IsActive", true))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property("AccountNumber"), nameof(CommandWalletReportVM.AccountNumber))
                .Add(Projections.Property("BankCode"), nameof(CommandWalletReportVM.BankCode))
                .Add(Projections.Property("Command.Name"), nameof(CommandWalletReportVM.CommandName))
                .Add(Projections.Property("Command.Code"), nameof(CommandWalletReportVM.CommandCode))
                .Add(Projections.Property("SettlementAccountType"), nameof(CommandWalletReportVM.SettlementAccountType)))
                .SetResultTransformer(Transformers.AliasToBean<CommandWalletReportVM>())
                .Future<CommandWalletReportVM>();
        }


        public ICriteria GetCriteria(CommandWalletReportSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<CommandWalletDetails>(nameof(CommandWalletDetails));
            criteria.CreateAlias("CommandWalletDetails.Command", "Command");

            foreach (var filter in _searchCommandReportFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}