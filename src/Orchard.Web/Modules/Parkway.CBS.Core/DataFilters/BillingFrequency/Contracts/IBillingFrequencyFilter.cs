using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.DataFilters.BillingFrequency.Contracts
{
    public interface IBillingFrequencyFilter : IDependency
    {
        /// <summary>
        /// Get frequency name
        /// </summary>
        /// <returns>FrequencyType</returns>
        FrequencyType Frequency();

        /// <summary>
        /// Validate fixed frequency
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="NoDaysSelectedForBillingFrequencyException"></exception>
        /// <exception cref="NoDayOfTheWeekAddedForBillingFrequencyException"></exception>
        /// <exception cref="NoMonthsAddedForBillingFrequencyException"></exception>
        /// <exception cref="NoYearsAddedForBillingFrequencyException"></exception>
        void ValidateFixed(FixedBillingModel model);

        /// <summary>
        /// Get cron expression
        /// </summary>
        /// <param name="model"></param>
        /// <param name="startDate">Date to start</param>
        /// <returns>string</returns>
        string GetCronExpression(FixedBillingModel model, DateTime startDate);

        /// <summary>
        /// Validate varibale billing info
        /// </summary>
        /// <param name="model">VariableBillingModel</param>
        void ValidateVariable(VariableBillingModel model);

        /// <summary>
        /// set a fixed frequency model
        /// </summary>
        /// <param name="fixedBill"></param>
        /// <param name="frequencyModel"></param>
        void SetFixedModel(FixedBillingModel fixedBill, BillingFrequencyModel frequencyModel);
    }
}
