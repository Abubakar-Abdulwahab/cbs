using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentReport
{
    public class AccountWalletPaymentReportFilter : IAccountWalletPaymentReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<Lazy<IAccountWalletPaymentReportFilters>> _accountWalletPaymentReportFilters;

        public AccountWalletPaymentReportFilter(IOrchardServices orchardService, IEnumerable<Lazy<IAccountWalletPaymentReportFilters>> accountWalletPaymentReportFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _accountWalletPaymentReportFilters = accountWalletPaymentReportFilters;
        }


        /// <summary>
        /// Get view model for Account Wallet payment report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalAccountWalletPaymentReportRecord }</returns>
        public dynamic GetAccountWalletPaymentReportViewModel(AccountWalletPaymentSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalAccountWalletPaymentReportRecord = GetTotalAccountWalletPaymentRecord(searchParams);
                return returnOBJ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get the total number of wallet payment requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        private IEnumerable<ReportStatsVM> GetTotalAccountWalletPaymentRecord(AccountWalletPaymentSearchParams searchParams)
        {
            return GetCriteria(searchParams)
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<AccountPaymentRequestItem>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
                    .Add(Projections.Sum<AccountPaymentRequestItem>(x => x.Amount), nameof(ReportStatsVM.TotalAmount))
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get the list of wallet payment requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns><see cref="IEnumerable{Command}"/></returns>
        private IEnumerable<AccountWalletPaymentReportVM> GetReport(AccountWalletPaymentSearchParams searchParams)
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
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.CreatedAtUtc)}"), nameof(AccountWalletPaymentReportVM.DateInitiated))
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.Id)}"), nameof(AccountWalletPaymentReportVM.AccountPaymentRequestId))
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.PaymentReference)}"), nameof(AccountWalletPaymentReportVM.PaymentId))
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.AccountNumber)}"), nameof(AccountWalletPaymentReportVM.SourceAccountNumber))
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.AccountName)}"), nameof(AccountWalletPaymentReportVM.SourceAccount))
                .Add(Projections.Property($"{nameof(AccountPaymentRequestItem.Bank)}.{nameof(AccountPaymentRequestItem.Bank.Name)}"), nameof(AccountWalletPaymentReportVM.Bank))
                  .Add(Projections.Property($"{nameof(AccountPaymentRequestItem)}.{nameof(AccountPaymentRequestItem.Amount)}"), nameof(AccountWalletPaymentReportVM.Amount))
                 .Add(Projections.Property($"{nameof(AccountPaymentRequestItem.PSSExpenditureHead)}.{nameof(AccountPaymentRequestItem.PSSExpenditureHead.Name)}"), nameof(AccountWalletPaymentReportVM.ExpenditureHead))
                .Add(Projections.Property($"{nameof(AccountPaymentRequestItem)}.{nameof(AccountPaymentRequestItem.AccountName)}"), nameof(AccountWalletPaymentReportVM.AccountName))
                .Add(Projections.Property($"{nameof(AccountPaymentRequestItem)}.{nameof(AccountPaymentRequestItem.TransactionStatus)}"), nameof(AccountWalletPaymentReportVM.Status))
                .Add(Projections.Property($"{nameof(AccountPaymentRequestItem)}.{nameof(AccountPaymentRequestItem.AccountNumber)}"), nameof(AccountWalletPaymentReportVM.AccountNumber))
                 .Add(Projections.Property($"{nameof(AccountPaymentRequestItem)}.{nameof(AccountPaymentRequestItem.BeneficiaryName)}"), nameof(AccountWalletPaymentReportVM.BeneficiaryName)))
                .AddOrder(Order.Desc($"{nameof(AccountPaymentRequestItem.CreatedAtUtc)}"))
                .SetResultTransformer(Transformers.AliasToBean<AccountWalletPaymentReportVM>())
                .Future<AccountWalletPaymentReportVM>();
        }

        public ICriteria GetCriteria(AccountWalletPaymentSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<AccountPaymentRequestItem>(nameof(AccountPaymentRequestItem))
            .CreateAlias($"{nameof(AccountPaymentRequestItem)}.{nameof(AccountPaymentRequestItem.AccountPaymentRequest)}", nameof(AccountPaymentRequestItem.AccountPaymentRequest)).CreateAlias($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.AccountWalletConfiguration)}", nameof(AccountPaymentRequest.AccountWalletConfiguration)).CreateAlias($"{nameof(AccountPaymentRequest.AccountWalletConfiguration)}.{nameof(AccountPaymentRequest.AccountWalletConfiguration.FlowDefinition)}", nameof(AccountPaymentRequest.AccountWalletConfiguration.FlowDefinition))
             .CreateAlias($"{nameof(AccountPaymentRequestItem)}.{nameof(AccountPaymentRequestItem.Bank)}", nameof(AccountPaymentRequestItem.Bank)).CreateAlias($"{nameof(AccountPaymentRequestItem)}.{nameof(AccountPaymentRequestItem.PSSExpenditureHead)}", nameof(AccountPaymentRequestItem.PSSExpenditureHead))
                .Add(Restrictions.Between(nameof(AccountPaymentRequestItem.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            if (searchParams.ApplyRestriction)
            {
                var requestFlowApproverCriteria = DetachedCriteria.For<PSServiceRequestFlowApprover>(nameof(PSServiceRequestFlowApprover))
            .CreateAlias($"{nameof(PSServiceRequestFlowApprover)}.{nameof(PSServiceRequestFlowApprover.FlowDefinitionLevel)}", "FDL")
            .CreateAlias($"{nameof(PSServiceRequestFlowApprover)}.{nameof(PSServiceRequestFlowApprover.AssignedApprover)}",
                nameof(PSServiceRequestFlowApprover.AssignedApprover))
            .Add(Restrictions.Eq($"{nameof(PSServiceRequestFlowApprover.AssignedApprover)}.{nameof(PSServiceRequestFlowApprover.AssignedApprover.Id)}", searchParams.UserPartRecordId))
            .Add(Restrictions.EqProperty($"FDL.Definition.Id", $"{nameof(AccountPaymentRequest.AccountWalletConfiguration.FlowDefinition)}.{nameof(AccountPaymentRequest.AccountWalletConfiguration.FlowDefinition.Id)}"))

            .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(requestFlowApproverCriteria));
            }


            foreach (var filter in _accountWalletPaymentReportFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}