using Parkway.CBS.Core.DataFilters.BillingFrequency.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.DataFilters.BillingFrequency
{
    public class WeeklyFrequency : BaseFrequency, IBillingFrequencyFilter
    {
        private readonly ICronGenerator _cronBaker;

        public WeeklyFrequency(ICronGenerator cronBaker) : base(cronBaker)
        {
            _cronBaker = cronBaker;
        }

        public FrequencyType Frequency()
        {
            return FrequencyType.Weekly;
        }

        public string GetCronExpression(FixedBillingModel model, DateTime startDate)
        {
            var days = model.WeeklyBill.Days.Select(d => d.ToString().Substring(0, 3)).ToList();
            return _cronBaker.SetWeekly(days, startDate.Month)
                .SetMinute(startDate.Minute).SetHour(startDate.Hour).SetYear(1, startDate.Year).Expression();
        }

        /// <summary>
        /// set the monthlyg frequency model
        /// </summary>
        /// <param name="fixedBill"></param>
        /// <param name="frequencyModel"></param>
        public void SetFixedModel(FixedBillingModel fixedBill, BillingFrequencyModel frequencyModel)
        {
            frequencyModel.FixedBill.WeeklyBill = new WeeklyBillingModel { Days = fixedBill.WeeklyBill.Days };
        }

        /// <summary>
        /// Validate fixed weeks
        /// </summary>
        /// <param name="model">BillingFrequencyModel</param>
        /// <exception cref="NoDayOfTheWeekAddedForBillingFrequencyException"></exception>
        /// <exception cref="NoDaysSelectedForBillingFrequencyException"></exception>
        public void ValidateFixed(FixedBillingModel model)
        {
                ValidateWeeks<WeeklyFrequency>(model.WeeklyBill);
        }

        /// <summary>
        /// Validate variable weeks
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="NoDayOfTheWeekAddedForBillingFrequencyException"></exception>
        public void ValidateVariable(VariableBillingModel model)
        {
            if (model == null || model.Interval <= 0)
            {
                throw new BillingFrequencyException(ErrorLang.noweeksinfrequencymodel().ToString());
            }
        }
    }
}