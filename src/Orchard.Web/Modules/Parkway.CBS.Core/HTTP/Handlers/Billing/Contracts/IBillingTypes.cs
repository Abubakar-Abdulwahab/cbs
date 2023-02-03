using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HTTP.Handlers.Billing.Contracts
{
    public interface IBillingTypes : IDependency
    {
        BillingType BillingType { get;}

        /// <summary>
        /// Validate billing model
        /// </summary>
        /// <param name="billingHelperModel"></param>
        void ValidateModel(BillingHelperModel billingHelperModel, ref List<ErrorModel> errors);


        /// <summary>
        /// Create billing model. 
        /// <para>
        /// Only call this method when you have validated the helper model
        /// </para>
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="revenueHead"></param>
        /// <param name="billingHelperModel"></param>
        /// <returns>BillingModel</returns>
        BillingModel CreateBilling(MDA mda, BillingHelperModel billingHelperModel, ref List<ErrorModel> errors);


        BillingSchedule CreateSchedule(BillingModel billing, RevenueHead revenueHead, MDA mda);


        void ScheduleJob(BillingModel billing);
    }
}
