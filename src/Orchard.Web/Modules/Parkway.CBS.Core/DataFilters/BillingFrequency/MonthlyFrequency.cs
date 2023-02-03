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
    public class MonthlyFrequency : BaseFrequency, IBillingFrequencyFilter
    {
        private readonly ICronGenerator _cronBaker;

        public MonthlyFrequency(ICronGenerator cronBaker) : base(cronBaker)
        {
            _cronBaker = cronBaker;
        }

        public FrequencyType Frequency()
        {
            return FrequencyType.Monthly;
        }

        public string GetCronExpression(FixedBillingModel model, DateTime startDate)
        {
            var months = model.MonthlyBill.Months.Select(m => m.ToString().Substring(0, 3)).ToList();
            return _cronBaker.SetMonths(months, (int)model.MonthlyBill.WeekDay, (int)model.MonthlyBill.SelectedDay)
                .SetMinute(startDate.Minute).SetHour(startDate.Hour).SetYear(1, startDate.Year).Expression();
        }

        /// <summary>
        /// set the billing model
        /// </summary>
        /// <param name="fixedBill"></param>
        /// <param name="frequencyModel"></param>
        public void SetFixedModel(FixedBillingModel fixedBill, BillingFrequencyModel frequencyModel)
        {
            frequencyModel.FixedBill.MonthlyBill = new MonthlyBillingModel { Months = fixedBill.MonthlyBill.Months, SelectedDay = fixedBill.MonthlyBill.SelectedDay, WeekDay = fixedBill.MonthlyBill.WeekDay };
        }

        /// <summary>
        /// Validate fixed month
        /// </summary>
        /// <param name="model"></param>
        public void ValidateFixed(FixedBillingModel model)
        {
            ValidateMonths<MonthlyFrequency>(model.MonthlyBill);
        }


        /// <summary>
        /// Validate variable frequency
        /// </summary>
        /// <param name="model"></param>
        public void ValidateVariable(VariableBillingModel model)
        {
            if (model == null || model.Interval <= 0)
            {
                throw new BillingFrequencyException(ErrorLang.nomonthsinfrequencymodel().ToString());
            }
        }
    }
}