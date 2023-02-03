using Parkway.CBS.Core.HTTP.Handlers.Billing.Contracts;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using System.Globalization;
using Parkway.CBS.Core.DataFilters.BillingFrequency.Contracts;
using Quartz;

namespace Parkway.CBS.Core.HTTP.Handlers.Billing
{
    public class Fixed : BaseBilling, IBillingTypes
    {
        private readonly IEnumerable<IBillingFrequencyFilter> _billingFrequencyFilter;
        public BillingType BillingType =>  BillingType.Fixed;

        public Fixed(IEnumerable<IBillingFrequencyFilter> billingFrequencyFilter)
        {
            _billingFrequencyFilter = billingFrequencyFilter;
        }


        public void ValidateModel(BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            Logger.Information("Validating fixed billing request");
            //validating amount
            if (billingHelperModel == null || billingHelperModel.BillingFrequencyModel == null || billingHelperModel.BillingFrequencyModel.FixedBill == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.nobillingtypefound().ToString(), FieldName = "BillingType" });
                throw new DirtyFormDataException();
            }

            if(billingHelperModel.AssessmentModel.Amount <= 0.00m)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.amountistoosmall().ToString(), FieldName = "SAmount" });
                throw new DirtyFormDataException();
            }

            if (billingHelperModel.Surcharge < 0.00m)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.surchargeAmtNotValid().ToString(), FieldName = "Surcharge" });
                throw new DirtyFormDataException();
            }

            if (billingHelperModel.BillingFrequencyModel.Duration == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.durationisrequired().ToString(), FieldName = "Duration" });
                throw new DirtyFormDataException();
            }
            //validate start time and date

            try
            {
                Logger.Information("validating start date and time");
                billingHelperModel.BillingFrequencyModel.FixedBill.StartDateAndTime = GetStartDateAndTime(billingHelperModel.BillingFrequencyModel.FixedBill);
            }
            catch (StartDateHasPassedException exception)
            {
                errors.Add(new ErrorModel { ErrorMessage = exception.Message, FieldName = "FixedBill.StartFrom",});
                throw new DirtyFormDataException();
            }
            catch(DateTimeCouldNotBeParsedException exception)
            {
                Logger.Error(exception, "Error parsing date time");
                errors.Add(new ErrorModel { FieldName = "FixedBill.StartFrom", ErrorMessage = "Date and time format is wrong. dd/MM/yyyy HH:mm" });
                throw new DirtyFormDataException();
            }

            try
            {
                Logger.Information("validating cron expression");
                billingHelperModel.BillingFrequencyModel.FixedBill.CRONExpression = ValidateAndGetCronExpression(billingHelperModel.BillingFrequencyModel.FrequencyType, billingHelperModel.BillingFrequencyModel.FixedBill, billingHelperModel.BillingFrequencyModel.FixedBill.StartDateAndTime);
            }
            catch (NoFrequencyTypeFoundException exception)
            {
                Logger.Error(exception, exception.StackTrace);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.notfrequencytypefound().ToString(), FieldName = "FrequencyType" });
                throw new DirtyFormDataException();
            }
            catch (BillingFrequencyException exception)
            {
                Logger.Error(exception, exception.StackTrace);
                errors.Add(new ErrorModel { ErrorMessage = exception.Message, FieldName = "Schedule" });
                throw new DirtyFormDataException();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.StackTrace);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "generic" });
                throw new DirtyFormDataException();
            }

            Logger.Information("validating next billing date for fixed billing " + billingHelperModel.BillingFrequencyModel.FixedBill.CRONExpression);
            try
            {
                billingHelperModel.BillingFrequencyModel.FixedBill.NextBillingDate = GetNextBillingDate(billingHelperModel.BillingFrequencyModel.FixedBill.CRONExpression, billingHelperModel.BillingFrequencyModel.FixedBill.StartDateAndTime);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.StackTrace + exception.Message + " Next billing date time could not be parsed for cron expression " + billingHelperModel.BillingFrequencyModel.FixedBill.CRONExpression);
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "generic" });
                throw new DirtyFormDataException();
            }

            Logger.Information("validating duration");
            //get the duration model
            try
            {
                billingHelperModel.BillingFrequencyModel.Duration = GetDuration(billingHelperModel.BillingFrequencyModel.Duration, billingHelperModel.BillingFrequencyModel.FixedBill.StartDateAndTime, ref errors);
            }
            catch(DirtyFormDataException) { throw; }
        }


        /// <summary>
        /// Validate before using this method
        /// <para>VALIDATE BEFORE USING THIS METHOD</para>
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="revenueHead"></param>
        /// <param name="billingHelperModel"></param>
        /// <param name="errors"></param>
        /// <returns>BillingModel</returns>
        public BillingModel CreateBilling(MDA mda, BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            Logger.Information(string.Format("Setting up create billing for direct assessment model : {0} ", JsonConvert.SerializeObject(billingHelperModel)));
            AssessmentModel assessment = new AssessmentModel();
            BillingFrequencyModel frequencyModel = new BillingFrequencyModel();
            DurationModel duration = new DurationModel();
            BillingModel billing = new BillingModel();

            Logger.Information("Setting billing type " + (int)BillingType);
            billing.BillingType = (int)BillingType.Fixed;

            frequencyModel.FrequencyType = billingHelperModel.BillingFrequencyModel.FrequencyType;
            frequencyModel.FixedBill = new FixedBillingModel();
            Logger.Information("Getting start date and time for fixed billing");

            frequencyModel.FixedBill.StartDateAndTime = billingHelperModel.BillingFrequencyModel.FixedBill.StartDateAndTime;
            //get cron expression
            Logger.Information("Getting cron expression for fixed billing");
            if (string.IsNullOrEmpty(billingHelperModel.BillingFrequencyModel.FixedBill.CRONExpression)) { throw new Exception("Validation was not done"); }
            frequencyModel.FixedBill.CRONExpression = billingHelperModel.BillingFrequencyModel.FixedBill.CRONExpression;
            //get next billing date
            Logger.Information("Getting next billing date for fixed billing " + frequencyModel.FixedBill.CRONExpression);
            billing.NextBillingDate = billingHelperModel.BillingFrequencyModel.FixedBill.NextBillingDate;
            Logger.Information("Getting duration");
            //get the duration model
            duration = billingHelperModel.BillingFrequencyModel.Duration;
            //set the freqency model.e.g if the frequency type is daily the daily model is instantiated
            Logger.Information("Setting fixed billing model");
            SetFixedBillingModel(billingHelperModel.BillingFrequencyModel.FrequencyType, billingHelperModel.BillingFrequencyModel.FixedBill, frequencyModel);

            assessment.IsRecurring = true;
            assessment.Amount = billingHelperModel.AssessmentModel.Amount;
            billing.Amount = billingHelperModel.AssessmentModel.Amount;
            billing.Surcharge = billingHelperModel.Surcharge;
            billing.Mda = mda;
            billing.BillingFrequency = JsonConvert.SerializeObject(frequencyModel);
            billing.Assessment = JsonConvert.SerializeObject(assessment);
            billing.Duration = JsonConvert.SerializeObject(duration);
            SetCommon(billingHelperModel, billing);
            return billing;
        }


        public BillingSchedule CreateSchedule(BillingModel billing, RevenueHead revenueHead, MDA mda)
        {
            Logger.Information("Setting up schedule for fixed billing");
            return new BillingSchedule
            {
                BillingModel = billing,
                RevenueHead = revenueHead,
                ScheduleStatus = (int)(revenueHead.IsActive ? ScheduleStatus.Running : ScheduleStatus.NotStarted),
                MDA = mda,
            };
        }


        public void ScheduleJob(BillingModel billing) { }
        
        
        /// <summary>
        /// Get start date and time
        /// </summary>
        /// <param name="validatedBillingHelperModel"></param>
        /// <returns>DateTime</returns>
        private DateTime GetStartDateAndTime(FixedBillingModel model)
        {
            DateTime startDate = new DateTime();
            try
            {
                startDate = DateTime.ParseExact(model.StartFrom.Trim() + " " + model.StartTime.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                //validate start date
                var dateNow = DateTime.Now.ToLocalTime();
                Logger.Error("Comparing start date " + dateNow.ToString());
                if (dateNow >= startDate) { throw new StartDateHasPassedException(); }
                return startDate;
            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(StartDateHasPassedException))
                {
                    Logger.Error(ErrorLang.datefrompast(startDate.ToString("dd'/'MM'/'yyyy")).ToString());
                    throw new StartDateHasPassedException(ErrorLang.datefrompast(startDate.ToString("dd'/'MM'/'yyyy")).ToString());
                }
                Logger.Error(exception, string.Format("Date and time could not be parsed. Date - {0} : Time - {1}", model.StartFrom, model.StartTime));
                throw new DateTimeCouldNotBeParsedException();
            }
        }


        /// <summary>
        /// Get cron expression
        /// </summary>
        /// <param name="frequencyType"></param>
        /// <param name="fixedBill"></param>
        /// <param name="startDate"></param>
        /// <returns>startDate</returns>
        public string ValidateAndGetCronExpression(FrequencyType frequencyType, FixedBillingModel fixedBill, DateTime startDate)
        {
            foreach (var freq in _billingFrequencyFilter)
            {
                if (freq.Frequency() == frequencyType)
                {
                    freq.ValidateFixed(fixedBill);
                    return freq.GetCronExpression(fixedBill, startDate);
                }
            }
            throw new NoFrequencyTypeFoundException();
        }


        /// <summary>
        /// Get datetime for a give cron expression
        /// </summary>
        /// <param name="cRONExpression"></param>
        /// <param name="fromDate"></param>
        /// <returns>DateTime</returns>
        /// <exception cref="Exception">If not date time could be gotten</exception>
        private DateTime GetNextBillingDate(string cRONExpression, DateTime fromDate)
        {
            var expression = new CronExpression(cRONExpression);
            var date = expression.GetNextValidTimeAfter(fromDate);
            return date.Value.LocalDateTime;
        }       


        /// <summary>
        /// set the frequency model
        /// </summary>
        /// <param name="frequencyType"></param>
        /// <param name="fixedBill"></param>
        /// <param name="frequencyModel"></param>
        private void SetFixedBillingModel(FrequencyType frequencyType, FixedBillingModel fixedBill, BillingFrequencyModel frequencyModel)
        {
            foreach (var freq in _billingFrequencyFilter)
            {
                if (freq.Frequency() == frequencyType)
                {
                    freq.SetFixedModel(fixedBill, frequencyModel);
                    return;
                }
            }
            throw new NoFrequencyTypeFoundException();
        }         
    }
}