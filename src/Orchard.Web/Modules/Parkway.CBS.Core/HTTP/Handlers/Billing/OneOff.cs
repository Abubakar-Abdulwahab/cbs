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
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.HTTP.Handlers.Billing
{
    public class OneOff : BaseBilling, IBillingTypes
    {
        public BillingType BillingType => BillingType.OneOff;

        public void ValidateModel(BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            Logger.Information("Validating amount " + billingHelperModel.AssessmentModel.Amount);

            if (billingHelperModel.AssessmentModel.Amount <= 0.00m)
            {
                Logger.Error("Amount is too");
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.amountistoosmall().ToString(), FieldName = "SAmount" });
                throw new DirtyFormDataException();
            }

            if (billingHelperModel.Surcharge < 0.00m)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.surchargeAmtNotValid().ToString(), FieldName = "Surcharge" });
                throw new DirtyFormDataException();
            }
        }


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
        public BillingModel CreateBilling(MDA mda, BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            Logger.Information(string.Format("Setting up create billing for one off model : {0} ", JsonConvert.SerializeObject(billingHelperModel)));
            BillingModel billing = new BillingModel();

            Logger.Information("Setting billing type " + billingHelperModel.BillingType.ToString());
            billing.BillingType = (int)BillingType;

            //the next billing date here is  inconsequential because it is only used to identify that it has been generated for a particular user
            billing.NextBillingDate = DateTime.Now.ToLocalTime();
            billing.Amount = billingHelperModel.AssessmentModel.Amount;
            billing.Surcharge = billingHelperModel.Surcharge;
            billing.Mda = mda;
            billing.BillingFrequency = JsonConvert.SerializeObject(new BillingFrequencyModel());
            billing.Assessment = JsonConvert.SerializeObject(new AssessmentModel { Amount = billingHelperModel.AssessmentModel.Amount });
            billing.Duration = JsonConvert.SerializeObject(new DurationModel());
            SetCommon(billingHelperModel, billing);

            return billing;
        }


        /// <summary>
        /// A schedule is not needed for oneoff billing
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="revenueHead"></param>
        /// <param name="mda"></param>
        /// <returns>BillingSchedule</returns>
        public BillingSchedule CreateSchedule(BillingModel billing, RevenueHead revenueHead, MDA mda) { return null; }


        /// <summary>
        /// No schedule is needed for oneoff billing
        /// </summary>
        /// <param name="billing"></param>
        public void ScheduleJob(BillingModel billing) { }
    }
}