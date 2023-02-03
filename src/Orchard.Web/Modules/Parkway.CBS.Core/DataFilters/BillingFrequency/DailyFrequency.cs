using Parkway.CBS.Core.DataFilters.BillingFrequency.Contracts;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using System.Runtime.Serialization;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Core.DataFilters.BillingFrequency
{
    public class DailyFrequency : BaseFrequency, IBillingFrequencyFilter
    {
        private readonly ICronGenerator _cronBaker;

        public DailyFrequency(ICronGenerator cronBaker) : base(cronBaker)
        {
            _cronBaker = cronBaker;
        }
       
        public FrequencyType Frequency()
        {
            return FrequencyType.Daily;
        }


        public string GetCronExpression(FixedBillingModel model, DateTime startDate)
        {
            return _cronBaker.SetDailyInterval(model.StartDateAndTime.Day, model.DailyBill.Interval)
                .SetMinute(startDate.Minute).SetHour(startDate.Hour).SetYear(1, startDate.Year).Expression();
        }


        /// <summary>
        /// set the billing model
        /// </summary>
        /// <param name="fixedBill"></param>
        /// <param name="frequencyModel"></param>
        public void SetFixedModel(FixedBillingModel fixedBill, BillingFrequencyModel frequencyModel)
        {
            frequencyModel.FixedBill.DailyBill = new DailyBillingModel() { Interval = fixedBill.DailyBill.Interval };
        }

        /// <summary>
        /// Make sure at least a day is selected
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="NoDaysSelectedForBillingFrequencyException"></exception>
        public void ValidateFixed(FixedBillingModel model)
        {
            ValidateDays<DailyFrequency>(model.DailyBill);
        }

        /// <summary>
        /// Validate variable frequency
        /// </summary>
        /// <param name="model"></param>
        public void ValidateVariable(VariableBillingModel model)
        {
            if (model == null || model.Interval <= 0)
            {
                throw new BillingFrequencyException(ErrorLang.daysintervalinfrequencymodelistoosmall().ToString());
            }
        }
    }    
}