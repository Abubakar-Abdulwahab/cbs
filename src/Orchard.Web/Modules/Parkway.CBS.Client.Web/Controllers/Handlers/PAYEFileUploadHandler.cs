using Orchard;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.IO;
using System.Web;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class PAYEFileUploadHandler : IPAYEFileUploadHandler
    {
        public ILogger Logger { get; set; }
        private readonly IPAYEHandler _payeHandler;
        private readonly IOrchardServices _orchardServices;

        public PAYEFileUploadHandler(IPAYEHandler payeHandler, IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _payeHandler = payeHandler;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Process file upload for PAYE schedule
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="userDetailsModel"></param>
        /// <param name="httpPostedFileBase"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public void DoProcessingForPAYEFileUpload(GenerateInvoiceStepsModel processStage, UserDetailsModel userDetails, HttpPostedFileBase file)
        {
            processStage.ProcessingDirectAssessmentReportVM = ProcessPAYEFileUploadAssessment(file, processStage, userDetails.Entity, userDetails.CBSUser);
        }

        /// <summary>
        /// Validates file input
        /// </summary>
        /// <param name="file"></param>
        /// <param name="errorMessage"></param>
        /// <exception cref="FileNotFoundException">File not found</exception>
        /// <exception cref="Exception">Invalid file type </exception>
        public void ValidateFileUpload(HttpPostedFileBase file, ref string errorMessage)
        {
            _payeHandler.ValidateFileUpload(file, ref errorMessage);
        }

        private ProcessingReportVM ProcessPAYEFileUploadAssessment(HttpPostedFileBase file, GenerateInvoiceStepsModel processStage, TaxEntity entity, CBSUser authorizedUser)
        {
            try
            {

                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                string fileName = Guid.NewGuid().ToString("N") + DateTime.Now.Ticks.ToString() + entity.Id + ".xls";

                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "/App_data/Media/PAYEAssessments/" + siteName);
                string path = Path.Combine(basePath.FullName, fileName);

                //Save file
                file.SaveAs(path);
                PAYEBatchRecordStaging batchRecordStaging = new PAYEBatchRecordStaging
                {
                    RevenueHead = new RevenueHead { Id = processStage.RevenueHeadId },
                    Billing = new BillingModel { Id = processStage.ProceedWithInvoiceGenerationVM.BillingId },
                    TaxEntity = entity,
                    CBSUser = authorizedUser,
                    AssessmentType = (int)PayeAssessmentType.FileUpload,
                    FilePath = path,
                };


                return _payeHandler.SaveBatchRecordStaging(batchRecordStaging, processStage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in file upload handler {0}", exception.Message));
                _payeHandler.RollBackAllTransactions();
                throw;
            }
        }

    }
}