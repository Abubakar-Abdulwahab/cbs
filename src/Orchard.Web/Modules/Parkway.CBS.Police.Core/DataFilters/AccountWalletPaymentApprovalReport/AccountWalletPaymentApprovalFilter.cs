using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentApprovalReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentApprovalReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentApprovalReport
{
    public class AccountWalletPaymentApprovalFilter : IAccountWalletPaymentApprovalFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<Lazy<IAccountWalletPaymentApprovalFilters>> _accountWalletPaymentFilters;

        public AccountWalletPaymentApprovalFilter(IOrchardServices orchardService, IEnumerable<Lazy<IAccountWalletPaymentApprovalFilters>> accountWalletPaymentFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _accountWalletPaymentFilters = accountWalletPaymentFilters;
        }

        /// <summary>
        /// Get view model for Account Wallet payment approval report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalAccountWalletPaymentApprovalRecord }</returns>
        public dynamic GetAccountWalletPaymentApprovalReportViewModel(AccountWalletPaymentApprovalSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalAccountWalletPaymentApprovalRecord = GetTotalAccountWalletPaymentApprovalRecord(searchParams);
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
        private IEnumerable<ReportStatsVM> GetTotalAccountWalletPaymentApprovalRecord(AccountWalletPaymentApprovalSearchParams searchParams)
        {
            return GetCriteria(searchParams)
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<AccountPaymentRequest>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get the list of wallet payment requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns><see cref="IEnumerable{Command}"/></returns>
        private IEnumerable<AccountWalletPaymentApprovalReportVM> GetReport(AccountWalletPaymentApprovalSearchParams searchParams)
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
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.CreatedAtUtc)}"), nameof(AccountWalletPaymentApprovalReportVM.DateInitiated))
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.Id)}"), nameof(AccountWalletPaymentApprovalReportVM.AccountPaymentRequestId))
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.PaymentReference)}"), nameof(AccountWalletPaymentApprovalReportVM.PaymentId))
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.AccountNumber)}"), nameof(AccountWalletPaymentApprovalReportVM.SourceAccountNumber))
                .Add(Projections.Property($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.AccountName)}"), nameof(AccountWalletPaymentApprovalReportVM.SourceAccount)))
                .AddOrder(Order.Desc($"{nameof(AccountPaymentRequest.CreatedAtUtc)}"))
                .SetResultTransformer(Transformers.AliasToBean<AccountWalletPaymentApprovalReportVM>())
                .Future<AccountWalletPaymentApprovalReportVM>();
        }

        public ICriteria GetCriteria(AccountWalletPaymentApprovalSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<AccountPaymentRequest>(nameof(AccountPaymentRequest));
            criteria.CreateAlias($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.FlowDefinitionLevel)}", nameof(AccountPaymentRequest.FlowDefinitionLevel)).CreateAlias($"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.AccountWalletConfiguration)}", nameof(AccountPaymentRequest.AccountWalletConfiguration))
              .CreateAlias($"{nameof(AccountPaymentRequest.FlowDefinitionLevel)}.{nameof(AccountPaymentRequest.FlowDefinitionLevel.Definition)}", nameof(AccountPaymentRequest.FlowDefinitionLevel.Definition))
              .Add(Restrictions.Eq($"{nameof(AccountPaymentRequest.FlowDefinitionLevel.Definition)}.{nameof(AccountPaymentRequest.FlowDefinitionLevel.Definition.DefinitionType)}", (int)DefinitionType.Payment))
              .Add(Restrictions.Between(nameof(AccountPaymentRequest.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate)).Add(Restrictions.Where<AccountPaymentRequest>(x => x.PaymentRequestStatus == (int)PaymentRequestStatus.AWAITINGAPPROVAL || x.PaymentRequestStatus == (int)PaymentRequestStatus.UNDERAPPROVAL));


            var requestFlowApproverCriteria = DetachedCriteria.For<PSServiceRequestFlowApprover>(nameof(PSServiceRequestFlowApprover))
            .CreateAlias($"{nameof(PSServiceRequestFlowApprover)}.{nameof(PSServiceRequestFlowApprover.FlowDefinitionLevel)}", "FDL")
            .CreateAlias($"{nameof(PSServiceRequestFlowApprover)}.{nameof(PSServiceRequestFlowApprover.AssignedApprover)}",
                nameof(PSServiceRequestFlowApprover.AssignedApprover))
            .Add(Restrictions.Eq($"{nameof(PSServiceRequestFlowApprover.AssignedApprover)}.{nameof(PSServiceRequestFlowApprover.AssignedApprover.Id)}", searchParams.UserPartRecordId))
            .Add(Restrictions.Eq($"FDL.WorkFlowActionValue", (int)RequestDirection.PaymentApproval))
            .Add(Restrictions.EqProperty($"FDL.Id", $"{nameof(AccountPaymentRequest)}.{nameof(AccountPaymentRequest.FlowDefinitionLevel)}.{nameof(AccountPaymentRequest.FlowDefinitionLevel.Id)}"))

            .SetProjection(Projections.Constant(1));
            criteria.Add(Subqueries.Exists(requestFlowApproverCriteria));

            foreach (var filter in _accountWalletPaymentFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }

    }
}