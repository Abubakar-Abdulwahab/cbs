using Parkway.CBS.Core.HTTP.Handlers.Billing.Contracts;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.DataFilters.BillingFrequency.Contracts;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Newtonsoft.Json;

namespace Parkway.CBS.Core.HTTP.Handlers.Billing
{
    public class Variable : BaseBilling, IBillingTypes
    {
        private readonly IEnumerable<IBillingFrequencyFilter> _billingFrequencyFilter;

        public BillingType BillingType => BillingType.Variable;


        public Variable(IEnumerable<IBillingFrequencyFilter> billingFrequencyFilter)
        {
            _billingFrequencyFilter = billingFrequencyFilter;
        }

        public void ValidateModel(BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            Logger.Information("Validating variable billing request");
            //validating amount
            if (billingHelperModel == null || billingHelperModel.BillingFrequencyModel == null || billingHelperModel.BillingFrequencyModel.VariableBill == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.nobillingtypefound().ToString(), FieldName = "BillingType" });
                throw new DirtyFormDataException();
            }

            if (billingHelperModel.AssessmentModel.Amount <= 0.00m)
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

            try
            {
                ValidateVariableBillingType(billingHelperModel.BillingFrequencyModel.VariableBill, billingHelperModel.BillingFrequencyModel.FrequencyType);
            }
            catch (NoFrequencyTypeFoundException exception)
            {
                Logger.Error(exception, "Error validating variable bill");
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.notfrequencytypefound().ToString(), FieldName = "Variable" });
                throw new DirtyFormDataException();
            }
            catch(BillingFrequencyException exception)
            {
                Logger.Error("Error validating interval");
                errors.Add(new ErrorModel { ErrorMessage = exception.Message, FieldName = "Variable" });
                throw new DirtyFormDataException();
            }

            Logger.Information("validating duration");
            //get the duration model
            try
            {
                billingHelperModel.BillingFrequencyModel.Duration = GetDuration(billingHelperModel.BillingFrequencyModel.Duration, new DateTime(), ref errors, true);
            }
            catch (DirtyFormDataException) { throw; }
        }

        public BillingModel CreateBilling(MDA mda, BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            Logger.Information(string.Format("Setting up create billing for direct assessment model : {0} ", JsonConvert.SerializeObject(billingHelperModel)));
            AssessmentModel assessment = new AssessmentModel();
            BillingFrequencyModel frequencyModel = new BillingFrequencyModel();
            DurationModel duration = new DurationModel();
            BillingModel billing = new BillingModel();

            Logger.Information("Setting billing type " + (int)BillingType);
            billing.BillingType = (int)BillingType.Variable;

            frequencyModel.FrequencyType = billingHelperModel.BillingFrequencyModel.FrequencyType;
            frequencyModel.VariableBill = new VariableBillingModel();
            Logger.Information("Getting duration");
            //get the duration model
            duration = billingHelperModel.BillingFrequencyModel.Duration;
            //set the freqency model.e.g if the frequency type is daily the daily model is instantiated
            frequencyModel.VariableBill.Interval = billingHelperModel.BillingFrequencyModel.VariableBill.Interval;
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


        public BillingSchedule CreateSchedule(BillingModel billing, RevenueHead revenueHead, MDA mda) { return null; }

        public void ScheduleJob(BillingModel billing) { }

        private void ValidateVariableBillingType(VariableBillingModel model, FrequencyType frequencyType)
        {
            foreach (var freq in _billingFrequencyFilter)
            {
                if (freq.Frequency() == frequencyType)
                {
                    freq.ValidateVariable(model);
                    return;
                }
            }
            throw new NoFrequencyTypeFoundException();
        }
    }
}