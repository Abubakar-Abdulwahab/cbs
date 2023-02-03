using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICorePaymentProviderValidationConstraintService : IDependency
    {

        /// <summary>
        /// Assign payment provider to selected revenue heads
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="admin"></param>
        void TryAssignPaymentProviderToRevenueHeads(AssignExternalPaymentProviderVM userInput, UserPartRecord admin);


        /// <summary>
        /// This method fetches existing access restrictions using the specified 
        /// for the payment provider Id.
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns>IEnumerable{PaymentProviderValidationConstraintsVM}</returns>
        IEnumerable<PaymentProviderValidationConstraintsVM> GetExistingRestrictions(int providerId);


        /// <summary>
        /// Synchronize constraints records in payment provider validation constraints table with MDA revenue access restrictions staging table.
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="mdaRevenueHeadEntryStagingReference"></param>
        void UpdatePaymentProviderConstraints(int providerId, string mdaRevenueHeadEntryStagingReference);


        /// <summary>
        /// Get the billable revenue heads
        /// </summary>
        /// <returns>List{MDAVM}</returns>
        List<MDAVM> GetMDAsForBillableRevenueHeads();


        /// <summary>
        /// Get payment provider 
        /// <para>ToFuture</para>
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns>IEnumerable{PaymentProviderVM}</returns>
        IEnumerable<PaymentProviderVM> GetProvider(int providerId);


        /// <summary>
        /// Get the revenue heads per MDA on this user Id access list
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="userId"></param>
        /// <param name="applyRestrictions"></param>
        /// <returns>IEnumerable{RevenueHeadLite}</returns>
        IEnumerable<RevenueHeadLite> GetRevenueHeadsPerMdaOnAccessList(int mdaId, int userId, bool applyRestrictions);


        /// <summary>
        /// Check if payment provider exists
        /// </summary>
        /// <param name="providerIdPased"></param>
        /// <returns>bool</returns>
        bool CheckIfPaymentProviderExists(int providerIdPased);

    }
}
