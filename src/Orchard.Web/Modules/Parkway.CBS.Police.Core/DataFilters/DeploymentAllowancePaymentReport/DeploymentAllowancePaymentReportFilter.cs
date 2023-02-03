using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.DataFilters.DeploymentAllowancePaymentReport.Contracts;
using Parkway.CBS.Police.Core.DataFilters.DeploymentAllowancePaymentReport.SearchFilters.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Parkway.CBS.Police.Core.DataFilters.DeploymentAllowancePaymentReport
{
    public class DeploymentAllowancePaymentReportFilter : IDeploymentAllowancePaymentReportFilter
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IEnumerable<Lazy<IDeploymentAllowancePaymentReportFilters>> _deploymentAllowancePaymentReportFilters;

        public DeploymentAllowancePaymentReportFilter(IOrchardServices orchardService, IEnumerable<Lazy<IDeploymentAllowancePaymentReportFilters>> deploymentAllowancePaymentReportFilters)
        {
            _transactionManager = orchardService.TransactionManager;
            _deploymentAllowancePaymentReportFilters = deploymentAllowancePaymentReportFilters;
        }


        /// <summary>
        /// Get view model for Deployment Allowance payment report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalDeploymentAllowancePaymentReportRecord }</returns>
        public dynamic GetDeploymentAllowancePaymentReportViewModel(DeploymentAllowancePaymentSearchParams searchParams)
        {
            dynamic returnOBJ = new ExpandoObject();

            try
            {
                returnOBJ.ReportRecords = GetReport(searchParams);
                returnOBJ.TotalDeploymentAllowancePaymentReportRecord = GetTotalDeploymentAllowancePaymentReportRecord(searchParams);
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
        private IEnumerable<ReportStatsVM> GetTotalDeploymentAllowancePaymentReportRecord(DeploymentAllowancePaymentSearchParams searchParams)
        {
            return GetCriteria(searchParams)
                .SetProjection(
               Projections.ProjectionList()
                    .Add(Projections.Count<DeploymentAllowancePaymentRequestItem>(x => x.Id), nameof(ReportStatsVM.TotalRecordCount))
                    .Add(Projections.Sum<DeploymentAllowancePaymentRequestItem>(x => x.Amount), nameof(ReportStatsVM.TotalAmount))
               ).SetResultTransformer(Transformers.AliasToBean<ReportStatsVM>()).Future<ReportStatsVM>();
        }

        /// <summary>
        /// Get the list of wallet payment requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns><see cref="IEnumerable{Command}"/></returns>
        private IEnumerable<DeploymentAllowancePaymentReportItemVM> GetReport(DeploymentAllowancePaymentSearchParams searchParams)
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
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequest)}.{nameof(DeploymentAllowancePaymentRequest.CreatedAtUtc)}"), nameof(DeploymentAllowancePaymentReportItemVM.DateInitiated))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequest)}.{nameof(DeploymentAllowancePaymentRequest.Id)}"), nameof(DeploymentAllowancePaymentReportItemVM.AccountPaymentRequestId))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequest)}.{nameof(DeploymentAllowancePaymentRequest.PaymentReference)}"), nameof(DeploymentAllowancePaymentReportItemVM.PaymentReference))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequest)}.{nameof(DeploymentAllowancePaymentRequest.AccountNumber)}"), nameof(DeploymentAllowancePaymentReportItemVM.SourceAccountNumber))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequest)}.{nameof(DeploymentAllowancePaymentRequest.AccountName)}"), nameof(DeploymentAllowancePaymentReportItemVM.SourceAccount))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequestItem)}.{nameof(DeploymentAllowancePaymentRequestItem.Amount)}"), nameof(DeploymentAllowancePaymentReportItemVM.Amount))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequestItem)}.{nameof(DeploymentAllowancePaymentRequestItem.TransactionStatus)}"), nameof(DeploymentAllowancePaymentReportItemVM.Status))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequestItem)}.{nameof(DeploymentAllowancePaymentRequestItem.AccountNumber)}"), nameof(DeploymentAllowancePaymentReportItemVM.AccountNumber))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequestItem)}.{nameof(DeploymentAllowancePaymentRequestItem.AccountName)}"), nameof(DeploymentAllowancePaymentReportItemVM.AccountName))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequestItem)}.{nameof(DeploymentAllowancePaymentRequestItem.StartDate)}"), nameof(DeploymentAllowancePaymentReportItemVM.StartDate))
                .Add(Projections.Property($"{nameof(DeploymentAllowancePaymentRequestItem)}.{nameof(DeploymentAllowancePaymentRequestItem.EndDate)}"), nameof(DeploymentAllowancePaymentReportItemVM.EndDate))
                .Add(Projections.Property($"{nameof(CommandType)}.{nameof(CommandType.Name)}"), nameof(DeploymentAllowancePaymentReportItemVM.CommandTypeName))
                .Add(Projections.Property($"{nameof(PSSEscortDayType)}.{nameof(PSSEscortDayType.Name)}"), nameof(DeploymentAllowancePaymentReportItemVM.DayTypeName))
                .Add(Projections.Property($"{nameof(TaxEntityProfileLocation)}.{nameof(TaxEntityProfileLocation.Name)}"), nameof(DeploymentAllowancePaymentReportItemVM.CustomerName)))
                .AddOrder(Order.Desc($"{nameof(DeploymentAllowancePaymentRequestItem.CreatedAtUtc)}"))
                .SetResultTransformer(Transformers.AliasToBean<DeploymentAllowancePaymentReportItemVM>())
                .Future<DeploymentAllowancePaymentReportItemVM>();
        }

        public ICriteria GetCriteria(DeploymentAllowancePaymentSearchParams searchParams)
        {
            var session = _transactionManager.GetSession();
            var criteria = session.CreateCriteria<DeploymentAllowancePaymentRequestItem>(nameof(DeploymentAllowancePaymentRequestItem))
            .CreateAlias($"{nameof(DeploymentAllowancePaymentRequestItem)}.{nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest)}", nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest))
            .CreateAlias($"{nameof(DeploymentAllowancePaymentRequest)}.{nameof(DeploymentAllowancePaymentRequest.AccountWalletConfiguration)}", nameof(DeploymentAllowancePaymentRequest.AccountWalletConfiguration))
            .CreateAlias($"{nameof(DeploymentAllowancePaymentRequest.AccountWalletConfiguration)}.{nameof(DeploymentAllowancePaymentRequest.AccountWalletConfiguration.FlowDefinition)}", nameof(DeploymentAllowancePaymentRequest.AccountWalletConfiguration.FlowDefinition))
            .CreateAlias($"{nameof(DeploymentAllowancePaymentRequestItem)}.{nameof(DeploymentAllowancePaymentRequestItem.CommandType)}", nameof(CommandType))
            .CreateAlias($"{nameof(DeploymentAllowancePaymentRequestItem)}.{nameof(DeploymentAllowancePaymentRequestItem.DayType)}", nameof(PSSEscortDayType))
            .CreateAlias($"{nameof(DeploymentAllowancePaymentRequest)}.{nameof(DeploymentAllowancePaymentRequest.PSSRequestInvoice)}", nameof(PSSRequestInvoice))
            .CreateAlias($"{nameof(PSSRequestInvoice)}.{nameof(PSSRequestInvoice.Request)}", nameof(PSSRequest))
            .CreateAlias($"{nameof(PSSRequest)}.{nameof(PSSRequest.TaxEntityProfileLocation)}", nameof(TaxEntityProfileLocation))
                .Add(Restrictions.Between(nameof(DeploymentAllowancePaymentRequestItem.CreatedAtUtc), searchParams.StartDate, searchParams.EndDate));

            if (searchParams.ApplyRestriction)
            {
                var requestFlowApproverCriteria = DetachedCriteria.For<PSServiceRequestFlowApprover>(nameof(PSServiceRequestFlowApprover))
            .CreateAlias($"{nameof(PSServiceRequestFlowApprover)}.{nameof(PSServiceRequestFlowApprover.FlowDefinitionLevel)}", "FDL")
            .CreateAlias($"{nameof(PSServiceRequestFlowApprover)}.{nameof(PSServiceRequestFlowApprover.AssignedApprover)}",
                nameof(PSServiceRequestFlowApprover.AssignedApprover))
            .Add(Restrictions.Eq($"{nameof(PSServiceRequestFlowApprover.AssignedApprover)}.{nameof(PSServiceRequestFlowApprover.AssignedApprover.Id)}", searchParams.UserPartRecordId))
            .Add(Restrictions.EqProperty($"FDL.Definition.Id", $"{nameof(DeploymentAllowancePaymentRequest.AccountWalletConfiguration.FlowDefinition)}.{nameof(DeploymentAllowancePaymentRequest.AccountWalletConfiguration.FlowDefinition.Id)}"))

            .SetProjection(Projections.Constant(1));
                criteria.Add(Subqueries.Exists(requestFlowApproverCriteria));
            }



            foreach (var filter in _deploymentAllowancePaymentReportFilters)
            {
                filter.Value.AddCriteriaRestriction(criteria, searchParams);
            }

            return criteria;
        }
    }
}