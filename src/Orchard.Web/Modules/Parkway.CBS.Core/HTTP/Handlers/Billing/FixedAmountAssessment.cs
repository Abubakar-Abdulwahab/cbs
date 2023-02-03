using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Billing.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Logging;
using Newtonsoft.Json;

namespace Parkway.CBS.Core.HTTP.Handlers.Billing
{
    public class FixedAmountAssessment : BaseBilling, IBillingTypes
    {
        public BillingType BillingType => BillingType.FixedAmountAssessment;


        public BillingModel CreateBilling(MDA mda, BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            Logger.Information(string.Format("Setting up create billing for FixedAmountAssessment model : {0} ", JsonConvert.SerializeObject(billingHelperModel)));
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

        public BillingSchedule CreateSchedule(BillingModel billing, RevenueHead revenueHead, MDA mda) { return null; }
        

        public void ScheduleJob(BillingModel billing) { }


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
    }
}