using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IPaymentProviderValidationConstraintManager<PaymentProviderValidationConstraint> : IDependency, IBaseManager<PaymentProviderValidationConstraint>
    {

        /// <summary>
        /// This method queries the database to check if the the combination of the
        /// mda, revenue head and payment provider has any stored record. It returns the count.
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="paymentProviderId"></param>
        /// <returns>int</returns>
        int CountNumberOfValidationRestrictions(int mdaId, int revenueHeadId, int paymentProviderId);

        /// <summary>
        /// This method fetches existing validation constraints for the payment provider with the specified Id.
        /// </summary>
        /// <param name="providerId"></param>
        /// <returns></returns>
        IEnumerable<PaymentProviderValidationConstraintsVM> GetExistingConstraints(int providerId);


        /// <summary>
        /// Update payment provider constraints records with changes on the MDARevenueAccessRestrictionsStaging table for the payment provider with specified Id using the specified MDARevenueHeadEntryStagingId.
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="MDARevenueHeadEntryStagingId"></param>
        void UpdateProviderRecords(int providerId, Int64 MDARevenueHeadEntryStagingId);

    }  
}
