using Parkway.CBS.Core.HTTP.Handlers.Billing.Contracts;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Payee;
using Newtonsoft.Json;
using Orchard.Logging;

namespace Parkway.CBS.Core.HTTP.Handlers.Billing
{
    public class DirectAssessment : BaseBilling, IBillingTypes
    {
        public BillingType BillingType { get => BillingType.DirectAssessment;  }


        /// <summary>
        /// Validate billing model
        /// </summary>
        /// <param name="billingHelperModel"></param>
        /// <param name="errors"></param>
        public void ValidateModel(BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            if (billingHelperModel.DirectAssessment != null && billingHelperModel.DirectAssessment.AllowFileUpload)
            {
                //check if a paye revenue head already exists
                if (billingHelperModel.DirectAssessment.AdapterValue == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.forfileuploadsavalidadapterisrequired().ToString(), FieldName = "DirectAssessment.AdapterValue" });
                    throw new DirtyFormDataException();
                }
                //if the direct assessment allows file upload, validate that the correct assessment adapter has been selected
                AssessmentInterface selectedInterface = billingHelperModel.DirectAssessment.DirectAssessmentAdapters.Where(inf => inf.Value == billingHelperModel.DirectAssessment.AdapterValue).FirstOrDefault();

                if (selectedInterface == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.forfileuploadsavalidadapterisrequired().ToString(), FieldName = "DirectAssessment.AdapterValue" });
                    throw new DirtyFormDataException();
                }
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
            Logger.Information(string.Format("Setting up create billing for direct assessment model : {0}", JsonConvert.SerializeObject(billingHelperModel)));
            AssessmentModel assessment = new AssessmentModel();
            BillingFrequencyModel frequencyModel = new BillingFrequencyModel();
            DurationModel duration = new DurationModel();
            BillingModel billing = new BillingModel();

            Logger.Information("Setting billing type " + (int)BillingType);
            billing.BillingType = (int)BillingType.DirectAssessment;

            //the next billing date here is  inconsequential direct asessments are to be generated for as many times the user request for them
            billing.NextBillingDate = DateTime.Now.ToLocalTime();
            assessment.Amount = 0.00m;
            assessment.IsDirectAssessment = true;
            billing.Amount = 0.00m;
            billing.Surcharge = billingHelperModel.Surcharge;
            billing.Mda = mda;
            billing.BillingFrequency = JsonConvert.SerializeObject(frequencyModel);
            billing.Assessment = JsonConvert.SerializeObject(assessment);
            billing.Duration = JsonConvert.SerializeObject(duration);
            string directJson = string.Empty;
            if(billingHelperModel.DirectAssessment != null)
            {

                billingHelperModel.DirectAssessment.DirectAssessmentAdapters = null;
                directJson = JsonConvert.SerializeObject(billingHelperModel.DirectAssessment);
            }
            billing.DirectAssessmentModel = string.IsNullOrEmpty(directJson) ? null: directJson;
            SetCommon(billingHelperModel, billing);

            return billing;
        }


        /// <summary>
        /// No schedule for direct assessments
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="revenueHead"></param>
        /// <param name="mda"></param>
        /// <returns>null</returns>
        public BillingSchedule CreateSchedule(BillingModel billing, RevenueHead revenueHead, MDA mda) { return null; }


        /// <summary>
        /// No job schedule for direct assessments
        /// </summary>
        /// <param name="billing"></param>
        public void ScheduleJob(BillingModel billing) { }
    }
}