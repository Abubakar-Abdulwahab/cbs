using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface ISettlementFeePartiesHandler : IDependency
    {
        /// <summary>
        /// Validates and Creates settlement fee part in <see cref="SettlementFeePartyConfiguration"/>
        /// </summary>
        /// <param name="errors"> Validation errors</param>
        /// <param name="model">User input model</param>
        void AddFeeParty(ref List<ErrorModel> errors, AddSettlementFeePartyVM model);

        /// <summary>
        /// Add/Edit settlement fee party
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        void EditSettlementFeeParty(ref List<ErrorModel> errors, PSSSettlementFeePartiesVM model);

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        void CheckForPermission(Orchard.Security.Permissions.Permission permission);

        /// <summary>
        /// Populates <see cref="AddSettlementFeePartyVM"/> for view
        /// </summary>
        /// <returns></returns>
        AddSettlementFeePartyVM GetAddSettlementFeePartyVM();

        /// <summary>
        /// Populates the VM for edit fee parties settlement
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementFeePartiesVM GetVMForEditParties(SettlementFeePartiesSearchParams searchParams);

        /// <summary>
        /// Get Reports VM for Settlement fee parties report
        /// </summary>
        /// <param name="feePartyReportSearchParams"></param>
        /// <returns></returns>
        SettlementFeePartiesVM GetSettlementFeePartiesReportVM(FeePartyReportSearchParams feePartyReportSearchParams);

        /// <summary>
        /// Gets view model for settlement fee parties report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSSettlementFeePartiesVM GetVMForReports(SettlementFeePartiesSearchParams searchParams);
    }
}
