using Newtonsoft.Json;
using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Payee;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreDirectAssessmentBatchRecord : CoreBaseService, ICoreDirectAssessmentBatchRecord
    {

        private readonly IOrchardServices _orchardServices;
        private readonly IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> _directAssessmentBatchRecordRepository;
        private readonly IUnreconciledPayePaymentsManager<UnreconciledPayePayments> _unreconciledPayePaymentsRepository;
        private readonly ITenantStateSettings<TenantCBSSettings> _tenantStateSettings;
        public readonly IPayeeAssessmentConfiguration _payeConfig;

        public Localizer T { get; set; }


        public CoreDirectAssessmentBatchRecord(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> directAssessmentBatchRecordRepository, ITenantStateSettings<TenantCBSSettings> tenantStateSettings, IPayeeAssessmentConfiguration payeConfig, IUnreconciledPayePaymentsManager<UnreconciledPayePayments> unreconciledPayePaymentsRepository) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            _directAssessmentBatchRecordRepository = directAssessmentBatchRecordRepository;
            _tenantStateSettings = tenantStateSettings;
            _payeConfig = payeConfig;
            _unreconciledPayePaymentsRepository = unreconciledPayePaymentsRepository;
        }


        /// <summary>
        /// Save a direct Assessment batch record
        /// <para>type is used to determine what directory the path to file is save in.
        /// For FileUpload: /App_data/Media/DirectAssessments/
        /// ProcessedFileUpload: /App_data/Media/DirectAssessments/ProcessedFile/
        /// FileUploadFromAPI: /App_data/Media/DirectAssessments/API/
        /// </para>
        /// </summary>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="userProfile"></param>
        /// <param name="amount"></param>
        /// <param name="adapter"></param>
        /// <param name="siteName"></param>
        /// <param name="type"></param>
        /// <returns>DirectAssessmentBatchRecord</returns>
        /// <exception cref="CouldNotSaveDirectAssessmentBatchRecordException"></exception>
        public DirectAssessmentBatchRecord SaveDirectAssessmentRecord(RevenueHeadDetails revenueHeadDetails, UserDetailsModel userProfile, decimal amount, string siteName, PayeAssessmentType type)
        {
            try
            {
                DirectAssessmentModel directAssessment = JsonConvert.DeserializeObject<DirectAssessmentModel>(revenueHeadDetails.Billing.DirectAssessmentModel);

                DirectAssessmentBatchRecord batchRecord = new DirectAssessmentBatchRecord
                {
                    RevenueHead = revenueHeadDetails.RevenueHead,
                    Billing = revenueHeadDetails.Billing,
                    TaxEntity = userProfile.Entity,
                    CBSUser = userProfile.CBSUser,
                    Type = type,
                    Amount = amount,
                    AdapterValue = directAssessment.AdapterValue,
                };

                if (!_directAssessmentBatchRecordRepository.Save(batchRecord))
                {
                    Logger.Error("Cannot save direct assessment batch record");
                    throw new CouldNotSaveDirectAssessmentBatchRecordException();
                }
                if (type == PayeAssessmentType.None) { throw new CouldNotSaveDirectAssessmentBatchRecordException("No payment Assessment type found"); }

                if (type == PayeAssessmentType.OnScreen)
                { batchRecord.BatchRef = Util.ZeroPadUp(batchRecord.Id.ToString(), 7, "PAO"); }
                else
                {
                    batchRecord.BatchRef = Util.ZeroPadUp(batchRecord.Id.ToString(), 7, "PAF");
                    string fileName = string.Format("batchRef-{0}__batchId-{1}.xls", batchRecord.BatchRef, batchRecord.Id);
                    DirectoryInfo basePath = null;

                    if (type == PayeAssessmentType.FileUpload)
                    {
                        //add to this directory so the file can be processed by either the app or file watcher
                        basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "/App_data/Media/DirectAssessments/" + siteName);
                    }
                    else if (type == PayeAssessmentType.ProcessedFileUpload)
                    {
                        //add this to this directory to indicate that files have been processed
                        basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "/App_data/Media/DirectAssessments/ProcessedFile/" + siteName);
                    }
                    else if (type == PayeAssessmentType.FileUploadFromAPI)
                    {
                        //add to this directory so files can be processed seperate from file uploads
                        basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "/App_data/Media/DirectAssessments/API/" + siteName);
                    }
                    string path = Path.Combine(basePath.FullName, fileName);
                    batchRecord.FileName = fileName;
                    batchRecord.FilePath = path;
                }
                return batchRecord;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error creating DirectAssessmentBatchRecord " + exception.Message);
                throw new CouldNotSaveDirectAssessmentBatchRecordException();
            }
        }


        /// <summary>
        /// Save the posted file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="type"></param>
        /// <exception cref="Exception"></exception>
        public void SaveFile(HttpPostedFileBase file, string path)
        {
            try
            { file.SaveAs(path); }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error saving Direct assessment batch record file in Path: {0}, Exception: {1}", path, exception.Message));
                throw;
            }
        }


        /// <summary>
        /// Validate uploaded file
        /// </summary>
        /// <param name="file"></param>
        /// <returns>List{ErrorModel}</returns>
        public List<ErrorModel> ValidateFile(HttpPostedFileBase file)
        {
            //getting file details
            if (file == null || file.ContentLength <= 0)
            {
                Logger.Error("File content is empty for file upload");
                return new List<ErrorModel> { { new ErrorModel { ErrorMessage = "File content is empty for file upload", FieldName = "assessmentfile" } } };
            }
            //saving file details
            Logger.Information("Validating DirectAssessmentBatchRecord file");
            List<ErrorModel> errors = new List<ErrorModel>();
            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();
            filesAndFileNames.Add(new UploadedFileAndName { Upload = file, UploadName = "assessmentfile" });
            CheckFileType(filesAndFileNames, errors, new List<string> { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel" }, new List<string> { "xlsx", "xls" });
            return errors;
        }


        /// <summary>
        /// Get the adapter value, read the file upload.
        /// <para>HeaderValidateObject is returned in the return object, check that the error value. 
        /// If true, there has been a mismatch in the header values, the error message is included in the HeaderValidateObject ErrorMessage prop.
        /// If HeaderValidateObject error prop is false the payes are returned
        /// </para>
        /// </summary>
        /// <param name="adapter">Computation Adapter</param>
        /// <param name="filePath">File path to the excel file</param>
        /// <returns>GetPayeResponse</returns>
        public GetPayeResponse GetPayes(AssessmentInterface adapter, string filePath)
        {
            //get the adapter
            Logger.Information(string.Format("processing payee assessment for {0} and adapter {1}", filePath, adapter.Name));

            Logger.Information(string.Format("Adapter gotten {0}. Getting implementation now", adapter.ClassName));
            IPayeeAdapter adpt = _payeConfig.GetAdapterImplementation(adapter);
            Logger.Information(string.Format("Impl gotten for adatper {0} {1}. Getting the list of payee model", adapter.ClassName, adapter.Value));
            List<PayeeAssessmentLineRecordModel> payes = new List<PayeeAssessmentLineRecordModel> { };
            try
            { return adpt.GetPayeeModels(filePath, Util.GetAppRemotePath(), adapter.StateName); }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error reading file {0}", exception.Message));
                throw;
            }
        }


        /// <summary>
        /// Get the assessment interface for this adapter value
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <returns>AssessmentInterface</returns>
        public AssessmentInterface GetDirectAssessmentAdapter(string adapterValue)
        {
            Logger.Information("Getiing tenant object. Calling db");
            TenantCBSSettings tenant = _tenantStateSettings.GetCollection(x => x.Id != 0).FirstOrDefault();
            if (tenant == null) { throw new TenantNotFoundException("Tenant setting not found"); }
            var assessmentInterface = _payeConfig.GetAssessmentType(adapterValue, tenant.Identifier);
            assessmentInterface.StateName = tenant.StateName;
            return assessmentInterface;
        }


        /// <summary>
        /// Get computation implementation
        /// </summary>
        /// <param name="adapter">AssessmentInterface</param>
        /// <returns>IPayeeAdapter</returns>
        public IPayeeAdapter GetAdapterImplementation(AssessmentInterface adapter)
        {
            return _payeConfig.GetAdapterImplementation(adapter);
        }


        /// <summary>
        /// Get the direct payee assessment for the given period
        /// </summary>
        /// <param name="payePeriod"></param>
        public DirectAssessmentBatchRecord GetPayeAssessmentByMonthAndYear(string agencyCode, PayeAssessmentType type, DateTime payePeriod)
        {
            switch (type)
            {
                case PayeAssessmentType.None:
                    break;
                case PayeAssessmentType.FileUpload:
                    break;
                case PayeAssessmentType.OnScreen:
                    break;
                case PayeAssessmentType.ProcessedFileUpload:
                    break;
                case PayeAssessmentType.FileUploadFromAPI:
                    break;
                case PayeAssessmentType.FileUploadForIPPIS:
                    //check the impl for IPPIS Parkway.CBS.ClientFileServices.Implementations.IPPIS.IPPISFileProcessor
                    //we are getting the batch record from the DirectAssessmentBatchRecord that matches the 
                    //month and the year coupled with the agency code and the assessment type
                    return _directAssessmentBatchRecordRepository.Get(x => ((x.Month == payePeriod.Month) && (x.Year == payePeriod.Year) && (x.TaxPayerCode == agencyCode) && (x.AssessmentType == (int)type)));
                default:
                    break;
            }
            throw new NotImplementedException("No impl for this PayeAssessmentType " + type.ToString());            
        }


        /// <summary>
        /// Save unreconcile paye payments
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <param name="unreconciledPayePayments"></param>
        public void SaveUnreconciledPayePayments(UnreconciledPayePayments unreconciledPayePayments)
        {
            try
            {
                if (!_unreconciledPayePaymentsRepository.Save(unreconciledPayePayments)) { throw new CouldNotSaveRecord(); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Error saving unreconciledPayePayments msg: {0}", exception.Message));
                _unreconciledPayePaymentsRepository.RollBackAllTransactions();
                throw;
            }
        }
    }
}