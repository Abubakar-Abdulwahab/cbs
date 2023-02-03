using Parkway.CBS.Core.DataFilters.BillingFrequency.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Core.DataFilters.BillingFrequency
{
    public class YearlyFrequency : BaseFrequency, IBillingFrequencyFilter
    {
        private readonly ICronGenerator _cronBaker;

        public YearlyFrequency(ICronGenerator cronBaker) : base(cronBaker)
        {
            _cronBaker = cronBaker;
        }

        public FrequencyType Frequency()
        {
            return FrequencyType.Yearly;
        }

        public string GetCronExpression(FixedBillingModel model, DateTime startDate)
        {
            var yearlyBill = model.YearlyBill;
            var months = yearlyBill.MonthlyBill.Months.Select(m => m.ToString().Substring(0, 3)).ToList();
            return _cronBaker.SetMonths(months, (int)yearlyBill.MonthlyBill.WeekDay, (int)yearlyBill.MonthlyBill.SelectedDay)
                .SetMinute(startDate.Minute).SetHour(startDate.Hour)
                .SetYear(yearlyBill.NumberOfYears, startDate.Year).Expression();
        }


        /// <summary>
        /// set the yearly frequency model
        /// </summary>
        /// <param name="fixedBill"></param>
        /// <param name="frequencyModel"></param>
        public void SetFixedModel(FixedBillingModel fixedBill, BillingFrequencyModel frequencyModel)
        {
            frequencyModel.FixedBill.YearlyBill = new YearlyBillingModel
            {
                MonthlyBill = new MonthlyBillingModel
                {
                    WeekDay = fixedBill.YearlyBill.MonthlyBill.WeekDay,
                    SelectedDay = fixedBill.YearlyBill.MonthlyBill.SelectedDay,
                    Months = fixedBill.YearlyBill.MonthlyBill.Months
                },
                NumberOfYears = fixedBill.YearlyBill.NumberOfYears
            };
        }

        /// <summary>
        /// Validate yearly fixed billing frequency
        /// </summary>
        /// <param name="model">BillingFrequencyModel</param>
        /// <exception cref="NoYearsAddedForBillingFrequencyException"></exception>
        /// <exception cref="NoMonthsAddedForBillingFrequencyException"></exception>
        /// <exception cref="NoDayOfTheWeekAddedForBillingFrequencyException"></exception>
        /// <exception cref="NoDaysSelectedForBillingFrequencyException"></exception>
        public void ValidateFixed(FixedBillingModel model)
        {
            ValidateYears<YearlyFrequency>(model.YearlyBill)
                .ValidateMonths<YearlyFrequency>(model.YearlyBill.MonthlyBill);
        }

        public void ValidateVariable(VariableBillingModel model)
        {
            if (model == null || model.Interval <= 0)
            {
                throw new BillingFrequencyException(ErrorLang.noyearsinfrequencymodel().ToString());
            }
        }
    }
}