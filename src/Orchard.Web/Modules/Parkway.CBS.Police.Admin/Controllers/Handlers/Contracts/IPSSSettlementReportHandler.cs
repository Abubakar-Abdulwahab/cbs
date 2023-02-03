using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSSettlementReportHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);


        /// <summary>
        /// Gets PSS Settlement Report Summary VM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementReportSummaryVM GetVMForReportSummary(PSSSettlementReportSearchParams searchParams);

        /// <summary>
        /// Gets PSS Settlement Report Invoices VM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementReportInvoicesVM GetVMForReportInvoices(PSSSettlementReportSearchParams searchParams);

        /// <summary>
        /// Gets PSS Settlement Report Fee Party Breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementReportPartiesBreakdownVM GetVMForReportPartyBreakdown(PSSSettlementReportPartyBreakdownSearchParams searchParams);

        /// <summary>
        /// Gets Id of PSS Settlement Batch with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        long GetSettlementBatchId(string batchRef);

        /// <summary>
        /// Gets PSS Settlement Batch Breakdown
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementReportBatchBreakdownVM GetVMForReportBatchBreakdown(PSSSettlementReportBatchBreakdownSearchParams searchParams);

        /// <summary>
        /// Gets settlement report breakdown vm
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementReportBreakdownVM GetSettlementReportBreakdownVM(PSSSettlementReportBreakdownSearchParams searchParams);

        /// <summary>
        /// Gets View Model for settlement report aggregate
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementReportAggregateVM GetVMForReportAggregate(PSSSettlementReportAggregateSearchParams searchParams);

        /// <summary>
        /// Gets View Model for settlement report party
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementReportPartyVM GetVMForReportParty(PSSSettlementReportPartySearchParams searchParams);
    }
}
