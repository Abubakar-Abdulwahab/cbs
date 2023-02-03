using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Services.PAYEBatchItems;
using Parkway.CBS.Services.PAYEBatchItems.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    /// <summary>
    /// Handles validation and saving payee assessment
    /// </summary>
    public class PAYEHandler : IPAYEHandler
    {
        private readonly IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> _payeBatchRecordStagingrepo;
        private readonly IBillingModelManager<BillingModel> _billingRepo;

        private readonly ICorePAYEService _corePAYEService;
        private readonly ICoreHelperService _corehelper;

        public PAYEHandler(IPAYEBatchRecordStagingManager<PAYEBatchRecordStaging> payeBatchRecordStagingrepo,
                           ICoreHelperService corehelper, ICorePAYEService corePAYEService, IBillingModelManager<BillingModel> billingRepo, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository)
        {
            Logger = NullLogger.Instance;
            _payeBatchRecordStagingrepo = payeBatchRecordStagingrepo;
            _corehelper = corehelper;
            _corePAYEService = corePAYEService;
            _billingRepo = billingRepo;
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// Validates file input
        /// </summary>
        /// <param name="file"></param>
        /// <param name="errorMessage"></param>
        /// <exception cref="FileNotFoundException">File not found</exception>
        /// <exception cref="Exception">Invalid file type </exception>
        public void ValidateFileUpload(HttpPostedFileBase file, ref string errorMessage)
        {
            if (file == null || file.ContentLength <= 0)
            {
                Logger.Error("File content is empty for file upload");
                throw new FileNotFoundException();
            }

            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();

            { filesAndFileNames.Add(new UploadedFileAndName { Upload = file, UploadName = file.FileName }); }
            List<ErrorModel> errors = new List<ErrorModel> { };


            int fileSize = 2048;

            string sFileSize = AppSettingsConfigurations.GetSettingsValue(AppSettingEnum.PayeExcelFileSize);

            if (!string.IsNullOrEmpty(sFileSize))
            {
                int.TryParse(sFileSize, out fileSize);
            }

            _corehelper.CheckFileSize(filesAndFileNames, errors, fileSize);
            _corehelper.CheckFileType(filesAndFileNames, errors, new List<string> { "xls", "xlsx" }, new List<string> { ".xls", ".xlsx" });

            if (errors.Count > 0)
            {
                errorMessage = string.Join("\n", errors.Select(x => x.ErrorMessage));
                throw new Exception(errorMessage);
            }

        }

        /// <summary>
        /// Save batch record staging
        /// </summary>
        /// <param name="batchRecordStaging"></param>
        /// <exception cref="CouldNotSaveRecord">Could not save record</exception>
        /// <returns>An object of the saved batch record staging </returns>
        public ProcessingReportVM SaveBatchRecordStaging(PAYEBatchRecordStaging batchRecordStaging, GenerateInvoiceStepsModel processStage)
        {
            AssessmentInterface adapter = GetAssessmentInterface(processStage);
            batchRecordStaging.AdapterValue = adapter.Value;

            if (!_payeBatchRecordStagingrepo.Save(batchRecordStaging))
            {
                _payeBatchRecordStagingrepo.RollBackAllTransactions();
                throw new CouldNotSaveRecord();
            }

            return new ProcessingReportVM { Type = (PayeAssessmentType)batchRecordStaging.AssessmentType, AdapterValue = adapter.Value, RequestToken = EncryptBatchToken(JsonConvert.SerializeObject(new FileProcessModel { Id = batchRecordStaging.Id })) };
        }

        /// <summary>
        /// Encrypts the value
        /// </summary>
        /// <param name="value"></param>
        /// <returns> A string of the encrypted value </returns>
        public string EncryptBatchToken(string value)
        {
            return Util.LetsEncrypt(value, AppSettingsConfigurations.EncryptionSecret);
        }

        /// <summary>
        /// Gets the Assessment interface
        /// </summary>
        /// <param name="processStage"></param>
        /// <returns> AssessmentInterface object </returns>
        public AssessmentInterface GetAssessmentInterface(GenerateInvoiceStepsModel processStage)
        {
            Logger.Information("Processing paye assessment");
            string payeModelString = _billingRepo.GetDirectAssessmentModel(processStage.ProceedWithInvoiceGenerationVM.BillingId);

            Logger.Information("Gotten billing and mda record");
            DirectAssessmentModel directAssessment = JsonConvert.DeserializeObject<DirectAssessmentModel>(payeModelString);

            AssessmentInterface adapter = _corePAYEService.GetDirectAssessmentAdapter(directAssessment.AdapterValue);
            return adapter;
        }

        /// <summary>
        /// Roll back all transactions
        /// </summary>
        public void RollBackAllTransactions()
        {
            _payeBatchRecordStagingrepo.RollBackAllTransactions();
        }

        /// <summary>
        /// Queues items for validation by the batch record Id using hangfire
        /// </summary>
        /// <param name="batchStagingRecordId">Batch record Id for the collection of items to be validated</param>
        /// <param name="tenantName">Tenant name</param>
        public void QueueItemsForValidation(string tenantName, long batchStagingRecordId)
        {
            new PAYEBatchItemsValidatorJob().ValidateItemsByBatchRecordId(tenantName, batchStagingRecordId);
        }
    }
}