using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletReport
{
    public class AccountWalletReportFilter : IAccountWalletReportFilter
    {
        private readonly IEnumerable<Lazy<IAccountWalletReportFilters>> _accountWalletReportFilters;

        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public AccountWalletReportFilter(IOrchardServices orchardService, IEnumerable<Lazy<IAccountWalletReportFilters>> accountWalletReportFilters)
        {
            Logger = NullLogger.Instance;
            _transactionManager = orchardService.TransactionManager;
            _accountWalletReportFilters = accountWalletReportFilters;
        }

        /// <summary>
        /// Get view model for account wallet reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalAccountWalletRecord }</returns>
        public dynamic GetAccountWalletReportViewModel(AccountWalletReportSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalAccountWalletRecord = GetTotalNumberOfAccountWallet(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IEnumerable<ReportStatsVM> GetTotalNumberOfAccountWallet(AccountWalletReportSearchParams searchParams)
        {

            var criteria = GetCriteria(searchParams);

            return criteria
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<AccountWalletConfiguration>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get the list of account wallets from <see cref="AccountWalletConfiguration"/>
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns><see cref="IEnumerable{HelperModels.AccountsWalletReportVM}"/></returns>
        private IEnumerable<HelperModels.AccountsWalletReportVM> GetReport(AccountWalletReportSearchParams searchParams)
        {

            ICriteria criteria = GetCriteria(searchParams);

            if (!searchParams.DontPageData)
            {
                criteria
                .SetFirstResult(searchParams.Skip)
                .SetMaxResults(searchParams.Take);
            }

            return criteria
                .AddOrder(Order.Desc(nameof(AccountWalletConfiguration.UpdatedAtUtc)))
                .SetProjection(Projections.ProjectionList()
                .Add(Projections.Property($"BankCWD.Name"), nameof(HelperModels.AccountsWalletReportVM.Bank))
                .Add(Projections.Property(nameof(AccountWalletConfiguration.Id)), nameof(HelperModels.AccountsWalletReportVM.AccountWalletId))
                .Add(Projections.Property($"{nameof(AccountWalletConfiguration.FlowDefinition)}.{nameof(AccountWalletConfiguration.FlowDefinition.Id)}"), nameof(HelperModels.AccountsWalletReportVM.FlowDefinitionId))
                .Add(Projections.Property($"{nameof(AccountWalletConfiguration.CommandWalletDetail)}.{nameof(AccountWalletConfiguration.CommandWalletDetail.AccountNumber)}"), nameof(HelperModels.AccountsWalletReportVM.AccountNumber))
                .Add(Projections.Property($"{nameof(AccountWalletConfiguration.CommandWalletDetail.Command)}.{nameof(AccountWalletConfiguration.CommandWalletDetail.Command.Name)}"), nameof(HelperModels.AccountsWalletReportVM.AccountName))
                .Add(Projections.Property($"{nameof(AccountWalletConfiguration.PSSFeeParty)}.{nameof(AccountWalletConfiguration.PSSFeeParty.AccountNumber)}"), nameof(HelperModels.AccountsWalletReportVM.FeePartyAccountNumber))
                 .Add(Projections.Property($"{nameof(AccountWalletConfiguration.PSSFeeParty.Bank)}.{nameof(AccountWalletConfiguration.PSSFeeParty.Bank.Name)}"), nameof(HelperModels.AccountsWalletReportVM.FeePartyBank))
                .Add(Projections.Property($"{nameof(AccountWalletConfiguration.PSSFeeParty)}.{nameof(AccountWalletConfiguration.PSSFeeParty.Name)}"), nameof(HelperModels.AccountsWalletReportVM.FeePartyAccountName)))
                .SetResultTransformer(Transformers.AliasToBean<HelperModels.AccountsWalletReportVM>())
                .Future<HelperModels.AccountsWalletReportVM>().ToList();

        }

        private ICriteria GetCriteria(AccountWalletReportSearchParams searchParams)
        {
            ICriteria criteria = _transactionManager.GetSession().CreateCriteria<AccountWalletConfiguration>(nameof(AccountWalletConfiguration))
            .CreateAlias($"{nameof(AccountWalletConfiguration)}.{nameof(AccountWalletConfiguration.FlowDefinition)}", $"{nameof(AccountWalletConfiguration.FlowDefinition)}").CreateAlias($"{nameof(AccountWalletConfiguration)}.{nameof(AccountWalletConfiguration.PSSFeeParty)}", $"{nameof(AccountWalletConfiguration.PSSFeeParty)}", NHibernate.SqlCommand.JoinType.LeftOuterJoin).CreateAlias($"{nameof(AccountWalletConfiguration)}.{nameof(AccountWalletConfiguration.CommandWalletDetail)}", $"{nameof(AccountWalletConfiguration.CommandWalletDetail)}", NHibernate.SqlCommand.JoinType.LeftOuterJoin).CreateAlias($"{nameof(AccountWalletConfiguration.CommandWalletDetail)}.{nameof(AccountWalletConfiguration.CommandWalletDetail.Command)}", $"{nameof(AccountWalletConfiguration.CommandWalletDetail.Command)}", NHibernate.SqlCommand.JoinType.LeftOuterJoin).CreateAlias($"{nameof(AccountWalletConfiguration.CommandWalletDetail)}.{nameof(AccountWalletConfiguration.CommandWalletDetail.Bank)}", "BankCWD", NHibernate.SqlCommand.JoinType.LeftOuterJoin).CreateAlias($"{nameof(AccountWalletConfiguration.PSSFeeParty)}.{nameof(AccountWalletConfiguration.PSSFeeParty.Bank)}", $"{nameof(AccountWalletConfiguration.PSSFeeParty.Bank)}", NHibernate.SqlCommand.JoinType.LeftOuterJoin);

            foreach (var filter in _accountWalletReportFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}