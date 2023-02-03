using Parkway.CBS.Core.HTTP.Handlers.Billing.Contracts;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Orchard.Logging;
using Newtonsoft.Json;

namespace Parkway.CBS.Core.HTTP.Handlers.Billing
{
    public class SelfAssessment : BaseBilling, IBillingTypes
    {
        public BillingType BillingType => BillingType.SelfAssessment;


        /// <summary>
        /// create billing model for self assessment
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="billingHelperModel"></param>
        /// <param name="errors"></param>
        /// <returns>BillingModel</returns>
        public BillingModel CreateBilling(MDA mda, BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            Logger.Information(string.Format("Setting up create billing for one off model : {0} ", JsonConvert.SerializeObject(billingHelperModel)));
            BillingModel billing = new BillingModel();

            Logger.Information("Setting billing type " + billingHelperModel.BillingType.ToString());
            billing.BillingType = (int)BillingType;

            //the next billing date here is  inconsequential because it is only used to identify that it has been generated for a particular user
            billing.NextBillingDate = DateTime.Now.ToLocalTime();
            billing.Amount = 0.00m;
            billing.Surcharge = billingHelperModel.Surcharge;
            billing.Mda = mda;
            billing.BillingFrequency = JsonConvert.SerializeObject(new BillingFrequencyModel());
            billing.Assessment = JsonConvert.SerializeObject(new AssessmentModel { Amount = 0.00m});
            billing.Duration = JsonConvert.SerializeObject(new DurationModel());
            SetCommon(billingHelperModel, billing);

            return billing;
        }


        /// <summary>
        /// if you need to create schdeule for self assessment do work here
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="revenueHead"></param>
        /// <param name="mda"></param>
        /// <returns></returns>
        public BillingSchedule CreateSchedule(BillingModel billing, RevenueHead revenueHead, MDA mda)
        { return null; }

        /// <summary>
        /// If self assesment has a schdule element do work here
        /// </summary>
        /// <param name="billing"></param>
        public void ScheduleJob(BillingModel billing)
        { /* no schedule needed */ }


        /// <summary>
        /// Do validation for a self assessments
        /// </summary>
        /// <param name="billingHelperModel"></param>
        /// <param name="errors"></param>
        public void ValidateModel(BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        { /*do nothing here*/ }
    }
}