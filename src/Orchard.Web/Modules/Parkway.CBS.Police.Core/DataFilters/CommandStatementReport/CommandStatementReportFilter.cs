using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.CommandStatementReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.CommandStatementReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.CommandStatementReport
{
    public class CommandStatementReportFilter : ICommandStatementReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<Lazy<ICommandStatementReportFilters>> _searchCommandStatementReportFilters;

        public CommandStatementReportFilter(IOrchardServices orchardService, IEnumerable<Lazy<ICommandStatementReportFilters>> searchCommandStatementReportFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _searchCommandStatementReportFilters = searchCommandStatementReportFilters;
        }

        /// <summary>
        /// Get veiw model for commands statement reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfStatementRecords }</returns>
        public dynamic GetCommandStatementReportViewModel(CommandStatementReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalNumberOfStatementRecords = GetTotalNumberStatementRecords(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get the total number of statement records
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalNumberStatementRecords(CommandStatementReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            return query
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<WalletStatement>(x => x.Id), "TotalRecordCount")
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }


        /// <summary>
        /// Get the statement records
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns><see cref="IEnumerable{Command}"/></returns>
        private IEnumerable<CommandWalletStatementVM> GetReport(CommandStatementReportSearchParams searchParams)
        {
            var query = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                query
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return query
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property("Id"), nameof(CommandWalletStatementVM.Id))
                .Add(Projections.Property("TransactionReference"), nameof(CommandWalletStatementVM.TransactionReference))
                .Add(Projections.Property("ValueDate"), nameof(CommandWalletStatementVM.ValueDate))
                .Add(Projections.Property("TransactionDate"), nameof(CommandWalletStatementVM.TransactionDate))
                .Add(Projections.Property("Narration"), nameof(CommandWalletStatementVM.Narration))
                .Add(Projections.Property("TransactionTypeId"), nameof(CommandWalletStatementVM.TransactionTypeId))
                .Add(Projections.Property("Amount"), nameof(CommandWalletStatementVM.Amount)))
                .AddOrder(Order.Desc("Id"))
                .SetResultTransformer(Transformers.AliasToBean<CommandWalletStatementVM>())
                .Future<CommandWalletStatementVM>();
        }


        public ICriteria GetCriteria(CommandStatementReportSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<WalletStatement>(nameof(WalletStatement));
            criteria.Add(Restrictions.Between(nameof(WalletStatement.TransactionDate), searchParams.StartDate, searchParams.EndDate));

            var commandWalletDetailsCriteria = DetachedCriteria.For<CommandWalletDetails>(nameof(CommandWalletDetails))
                .CreateAlias($"{nameof(CommandWalletDetails)}.{nameof(CommandWalletDetails.Command)}", nameof(Command))
                .Add(Restrictions.EqProperty($"{nameof(WalletStatement)}.{nameof(WalletStatement.WalletId)}", $"{nameof(CommandWalletDetails)}.{nameof(CommandWalletDetails.Id)}"))
                .Add(Restrictions.Eq($"{nameof(WalletStatement)}.{nameof(WalletStatement.WalletIdentifierType)}", (int)Models.Enums.WalletIdentifierType.CommandWalletDetails))
                .Add(Restrictions.Eq($"{nameof(Command)}.{nameof(Command.Code)}", searchParams.CommandCode))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Constant(1)));

            foreach (var filter in _searchCommandStatementReportFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            criteria.Add(Subqueries.Exists(commandWalletDetailsCriteria));

            return criteria;
        }
    }
}