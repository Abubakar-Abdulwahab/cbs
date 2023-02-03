using Orchard;
using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreBillingScheduleService : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        bool IsScheduleDurationValid(BillingSchedule schedule, DurationModel duration);

        /// <summary>
        /// check if schedule can run
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        bool CanScheduleRun(BillingSchedule schedule);

        /// <summary>
        /// Get schedule for this bill
        /// </summary>
        /// <param name="billing">BillingModel</param>
        /// <returns>BillingSchedule</returns>
        BillingSchedule GetSchedule(BillingModel billing);

        List<BillingSchedule> TryCreateVariableSchedules(MDA mda, RevenueHead revenueHead, BillingModel billing, IEnumerable<TaxEntityInvoice> validatedTaxPayers, DateTime startDate);


        /// <summary>
        /// Get schedules for each tax payer
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="taxPayers"></param>
        /// <returns></returns>
        IEnumerable<BillingSchedule> GetSchedules(BillingModel billing, IEnumerable<TaxEntity> taxPayers);


        /// <summary>
        /// Create a billing schedule for fixed types
        /// </summary>
        /// <param name="revenueHead">Revenue head</param>
        /// <param name="billing">BillingModel</param>
        //void RegisterAFixedBillingSchedule(CBSTenantSettings tenant, RevenueHead revenueHead, BillingModel billing, BillingType billingType);

        void RegisterAFixedBillingSchedule(ExpertSystemSettings tenant, RevenueHead revenueHead, ScheduleHelperModel helper, BillingType billingType);
    }
}
