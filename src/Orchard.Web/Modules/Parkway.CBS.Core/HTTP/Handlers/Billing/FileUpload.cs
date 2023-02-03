using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Billing.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers.Billing
{
    public class FileUpload : BaseBilling, IBillingTypes
    {
        public BillingType BillingType => BillingType.FileUpload;


        /// <summary>
        /// create billing model for self assessment
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="billingHelperModel"></param>
        /// <param name="errors"></param>
        /// <returns>BillingModel</returns>
        public BillingModel CreateBilling(MDA mda, BillingHelperModel billingHelperModel, ref List<ErrorModel> errors)
        {
            Logger.Information(string.Format("Setting up create billing for File Upload model : {0} ", JsonConvert.SerializeObject(billingHelperModel)));
            BillingModel billing = new BillingModel();

            Logger.Information("Setting billing type " + billingHelperModel.BillingType.ToString());
            billing.BillingType = (int)BillingType;

            //the next billing date here is  inconsequential because it is only used to identify that it has been generated for a particular user
            billing.NextBillingDate = DateTime.Now.ToLocalTime();
            billing.Amount = 0.00m;
            billing.Surcharge = billingHelperModel.Surcharge;
            billing.Mda = mda;
            billing.BillingFrequency = JsonConvert.SerializeObject(new BillingFrequencyModel());
            billing.Assessment = JsonConvert.SerializeObject(new AssessmentModel { Amount = 0.00m });
            billing.Duration = JsonConvert.SerializeObject(new DurationModel());
            billing.FileUploadModel = JsonConvert.SerializeObject(new { SelectedTemplate = billingHelperModel.FileUploadTemplates.SelectedTemplate, SelectedImplementation = billingHelperModel.FileUploadTemplates.SelectedImplementation });
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
        {
            if (string.IsNullOrEmpty(billingHelperModel.FileUploadTemplates.SelectedTemplate))
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.valuerequired("Template").ToString(), FieldName = "FileUploadBillingModel.SelectedTemplate" });
                return;
            }

            if (string.IsNullOrEmpty(billingHelperModel.FileUploadTemplates.SelectedImplementation))
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.valuerequired("Adapter implementation").ToString(), FieldName = "FileUploadBillingModel.SelectedImplementation" });
                return;
            }

            var templateValue = billingHelperModel.FileUploadTemplates.ListOfTemplates.Where(templ => templ.Name == billingHelperModel.FileUploadTemplates.SelectedTemplate).FirstOrDefault();
            //validate that the selected tenplate and implementation are avaliable
            if (templateValue == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.notemplatefound(billingHelperModel.FileUploadTemplates.SelectedTemplate).ToString(), FieldName = "FileUploadBillingModel.SelectedTemplate" });
                return;
            }

            if (!templateValue.ListOfUploadImplementations.Any(impl => impl.Value == billingHelperModel.FileUploadTemplates.SelectedImplementation))
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.notemplateimplfound(billingHelperModel.FileUploadTemplates.SelectedImplementation).ToString(), FieldName = "FileUploadBillingModel.SelectedImplementation" });
                return;
            }
        }

    }
}