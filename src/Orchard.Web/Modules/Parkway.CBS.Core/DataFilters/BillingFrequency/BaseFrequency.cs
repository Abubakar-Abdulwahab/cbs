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
    public abstract class BaseFrequency
    {
        public FrequencyType frequencyTye;
        private readonly ICronGenerator _cronBaker;

        public BaseFrequency(ICronGenerator cronBaker)
        {
            _cronBaker = cronBaker;
        }


        /// <summary>
        /// Validate days
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NoDaysSelectedForBillingFrequencyException"></exception>
        protected T ValidateDays<T>(DailyBillingModel model) where T : BaseFrequency
        {
            //check days
            if (model == null || model.Interval <= 0)
            {
                throw new BillingFrequencyException(ErrorLang.daysintervalinfrequencymodelistoosmall().ToString());
            }
            return (T)this;
        }

        /// <summary>
        /// Check weeks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NoDayOfTheWeekAddedForBillingFrequencyException"></exception>
        protected T ValidateWeeks<T>(WeeklyBillingModel model) where T : BaseFrequency
        {
            if (model == null || model.Days == null || model.Days.Count <= 0)
            {
                throw new BillingFrequencyException(ErrorLang.nodaysinfrequencymodel().ToString());
            }
            return (T)this;
        }

        /// <summary>
        /// Validate months
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NoMonthsAddedForBillingFrequencyException"></exception>
        protected T ValidateMonths<T>(MonthlyBillingModel model) where T : BaseFrequency
        {
            //check months
            if (model == null || model.Months == null || model.Months.Count <= 0)
            { throw new BillingFrequencyException(ErrorLang.nomonthsinfrequencymodel().ToString()); }

            if (model.WeekDay == WeekDays.None) { throw new BillingFrequencyException(ErrorLang.noweeksinfrequencymodel().ToString()); }
            bool isWeekDay = model.WeekDay == WeekDays.FirstDayOfTheMonth || model.WeekDay == WeekDays.FirstWeekDayOfTheMonth || model.WeekDay == WeekDays.LastDayOfWeekDayOfTheMonth || model.WeekDay == WeekDays.LastDayOfTheMonth;
            if (model.SelectedDay == Days.None && !isWeekDay) { throw new BillingFrequencyException(ErrorLang.nodaysinfrequencymodel().ToString()); }

            return (T)this;
        }

        /// <summary>
        /// Check years
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NoYearsAddedForBillingFrequencyException"></exception>
        protected T ValidateYears<T>(YearlyBillingModel model) where T : BaseFrequency
        {
            //check years
            if (model == null || model.NumberOfYears <= 0)
            {
                throw new BillingFrequencyException(ErrorLang.noyearsinfrequencymodel().ToString());
            }
            return (T)this;
        }
    }
}