using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.DataFilters.GenerateRequestWithoutOfficersUploadBatchStagingReport.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System.IO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Newtonsoft.Json;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class GenerateRequestWithoutOfficersHandler : IGenerateRequestWithoutOfficersHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;
        private readonly IGenerateRequestWithoutOfficersUploadBatchStagingFilter _generateRequestWithoutOfficersUploadBatchStagingFilter;
        private readonly IGenerateRequestWithoutOfficersUploadBatchItemsStagingFilter _generateRequestWithoutOfficersUploadBatchItemsStagingFilter;
        private readonly ITaxEntityProfileLocationManager<TaxEntityProfileLocation> _taxEntityProfileLocationManager;
        private readonly ICoreHelperService _corehelper;
        private readonly IGenerateRequestWithoutOfficersUploadBatchStagingManager<GenerateRequestWithoutOfficersUploadBatchStaging> _generateRequestWithoutOfficersUploadBatchStagingManager;
        ILogger Logger { get; set; }
        public GenerateRequestWithoutOfficersHandler(IHandlerComposition handlerComposition, IOrchardServices orchardServices, ITaxEntityManager<TaxEntity> taxEntityManager, IGenerateRequestWithoutOfficersUploadBatchItemsStagingFilter generateRequestWithoutOfficersUploadBatchItemsStagingFilter, IGenerateRequestWithoutOfficersUploadBatchStagingFilter generateRequestWithoutOfficersUploadBatchStagingFilter, ITaxEntityProfileLocationManager<TaxEntityProfileLocation> taxEntityProfileLocationManager, ICoreHelperService corehelper, IGenerateRequestWithoutOfficersUploadBatchStagingManager<GenerateRequestWithoutOfficersUploadBatchStaging> generateRequestWithoutOfficersUploadBatchStagingManager)
        {
            _taxEntityManager = taxEntityManager;
            _handlerComposition = handlerComposition;
            _orchardServices = orchardServices;
            _generateRequestWithoutOfficersUploadBatchStagingFilter = generateRequestWithoutOfficersUploadBatchStagingFilter;
            _generateRequestWithoutOfficersUploadBatchItemsStagingFilter = generateRequestWithoutOfficersUploadBatchItemsStagingFilter;
            _taxEntityProfileLocationManager = taxEntityProfileLocationManager;
            _corehelper = corehelper;
            _generateRequestWithoutOfficersUploadBatchStagingManager = generateRequestWithoutOfficersUploadBatchStagingManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }


        /// <summary>
        /// Gets GenerateRequestForDefaultBranchWithoutOfficersUploadVM
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public GenerateRequestForDefaultBranchWithoutOfficersUploadVM GetGenerateRequestForDefaultBranchWithoutOfficersUploadVM(string payerId, GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            try
            {
                long taxEntityId = _taxEntityManager.GetTaxEntityId(x => x.PayerId == payerId);
                if (taxEntityId == 0) { return null; }
                TaxEntityProfileLocationVM defaultBranch = _taxEntityProfileLocationManager.GetDefaultTaxEntityLocation(taxEntityId);
                if (defaultBranch == null) { return null; }
                searchParams.TaxEntityProfileLocationId = defaultBranch.Id;
                dynamic recordsAndAggregate = _generateRequestWithoutOfficersUploadBatchStagingFilter.GetReportViewModel(searchParams);
                IEnumerable<GenerateRequestWithoutOfficersUploadBatchStagingDTO> records = ((IEnumerable<GenerateRequestWithoutOfficersUploadBatchStagingDTO>)recordsAndAggregate.ReportRecords);

                return new GenerateRequestForDefaultBranchWithoutOfficersUploadVM
                {
                    BranchDetails = defaultBranch,
                    GenerateRequestWithoutOfficersUploadBatches = (records == null || !records.Any()) ? Enumerable.Empty<GenerateRequestWithoutOfficersUploadBatchStagingDTO>() : records,
                    TotalRecordCount = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount
                };
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets GenerateRequestForBranchWithoutOfficersUploadVM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public GenerateRequestForBranchWithoutOfficersUploadVM GetGenerateRequestForBranchWithoutOfficersUploadVM(GenerateRequestWithoutOfficersUploadBatchStagingReportSearchParams searchParams)
        {
            try
            {
                dynamic recordsAndAggregate = _generateRequestWithoutOfficersUploadBatchStagingFilter.GetReportViewModel(searchParams);
                IEnumerable<GenerateRequestWithoutOfficersUploadBatchStagingDTO> records = (IEnumerable<GenerateRequestWithoutOfficersUploadBatchStagingDTO>)recordsAndAggregate.ReportRecords;

                var taxEntityProfileLocationVM = _taxEntityProfileLocationManager.GetTaxEntityLocationWithId(searchParams.TaxEntityProfileLocationId);

                return new GenerateRequestForBranchWithoutOfficersUploadVM
                {
                    DefaultBranchTaxEntityProfileLocation = _taxEntityProfileLocationManager.GetDefaultTaxEntityLocation(taxEntityProfileLocationVM.TaxEntity.Id),
                    BranchTaxEntityProfileLocation = taxEntityProfileLocationVM,
                    GenerateRequestWithoutOfficersUploadBatches = (records == null || !records.Any()) ? Enumerable.Empty<GenerateRequestWithoutOfficersUploadBatchStagingDTO>() : records,
                    TotalRecordCount = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount
                };
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Validates file size and type
        /// </summary>
        /// <param name="generateRequestWithoutOfficersUploadFile"></param>
        /// <param name="errors"></param>
        /// <returns>returns true if there are errors</returns>
        public bool ValidateGenerateRequestWithoutOfficersUploadFile(HttpPostedFileBase generateRequestWithoutOfficersUploadFile, ref List<ErrorModel> errors)
        {
            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.PSSEGSGenerateRequestWithoutOfficersUploadFileSize)).FirstOrDefault();
            int fileSizeCap = 0;
            if (string.IsNullOrEmpty(node?.Value) || !int.TryParse(node.Value, out fileSizeCap))
            {
                Logger.Error(string.Format("Unable to get pss admin generate request without officers upload file size in config file"));
                errors.Add(new ErrorModel { ErrorMessage = "Error uploading file.", FieldName = "generateRequestWithoutOfficersUploadFile" });
                return true;
            }
            filesAndFileNames.Add(new UploadedFileAndName { Upload = generateRequestWithoutOfficersUploadFile, UploadName = "generateRequestWithoutOfficersUploadFile" });
            _corehelper.CheckFileSize(filesAndFileNames, errors, fileSizeCap);

            return errors.Any();
        }


        /// <summary>
        /// Process generate request without officers upload file, saving and returning the batch token
        /// </summary>
        /// <param name="generateRequestWithoutOfficersUploadFile"></param>
        /// <param name="payerId"></param>
        /// <returns></returns>
        public string ProcessGenerateRequestWithoutOfficersUploadFileForEntity(HttpPostedFileBase generateRequestWithoutOfficersUploadFile, string payerId)
        {
            try
            {
                int taxEntityProfileLocationId = _taxEntityProfileLocationManager.GetDefaultTaxEntityLocationId(payerId);
                if (taxEntityProfileLocationId == 0)
                {
                    throw new Exception($"No default tax entity profile location found for user with payer id {payerId}");
                }
                string fileName = Util.StrongRandomNoSpecailCharacters() + DateTime.Now.Ticks.ToString() + payerId + Path.GetExtension(generateRequestWithoutOfficersUploadFile.FileName);
                return ProcessGenerateRequestWithoutOfficersUploadFile(generateRequestWithoutOfficersUploadFile, taxEntityProfileLocationId, fileName);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Process generate request without officers upload file, saving and returning the batch token
        /// </summary>
        /// <param name="generateRequestWithoutOfficersUploadFile"></param>
        /// <param name="payerId"></param>
        /// <returns></returns>
        public string ProcessGenerateRequestWithoutOfficersUploadFileForBranch(HttpPostedFileBase generateRequestWithoutOfficersUploadFile, int branchId)
        {
            try
            {
                if (_taxEntityProfileLocationManager.Count(x => x.Id == branchId) == 0)
                {
                    throw new Exception($"No tax entity profile location with id {branchId} found");
                }
                string fileName = Util.StrongRandomNoSpecailCharacters() + DateTime.Now.Ticks.ToString() + branchId + Path.GetExtension(generateRequestWithoutOfficersUploadFile.FileName);
                return ProcessGenerateRequestWithoutOfficersUploadFile(generateRequestWithoutOfficersUploadFile, branchId, fileName);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets GenerateRequestWithoutOfficersFileUploadValidationResultVM
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public GenerateRequestWithoutOfficersFileUploadValidationResultVM GetUploadValidationResult(string batchToken, GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams searchParams)
        {
            try
            {
                searchParams.GenerateRequestWithoutOfficersUploadBatchStagingId = GetFileProcessModel(batchToken).Id;
                if (_generateRequestWithoutOfficersUploadBatchStagingManager.Count(x => x.Id == searchParams.GenerateRequestWithoutOfficersUploadBatchStagingId) == 0)
                {
                    throw new Exception($"Generate request without officers upload file batch with Id {searchParams.GenerateRequestWithoutOfficersUploadBatchStagingId} not found.");
                }

                GenerateRequestWithoutOfficersUploadBatchStagingDTO generateRequestWithoutOfficersUploadBatch = _generateRequestWithoutOfficersUploadBatchStagingManager.GetGenerateRequestWithoutOfficersUploadBatchWithId(searchParams.GenerateRequestWithoutOfficersUploadBatchStagingId);
                dynamic recordsAndAggregate = _generateRequestWithoutOfficersUploadBatchItemsStagingFilter.GetReportViewModel(searchParams);
                IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> records = ((IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO>)recordsAndAggregate.ReportRecords);
                FileUploadReport uploadReport = ((IEnumerable<FileUploadReport>)recordsAndAggregate.ValidItemsAggregate).First();
                uploadReport.NumberOfRecords = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount;
                return new GenerateRequestWithoutOfficersFileUploadValidationResultVM
                {
                    BatchDetails = generateRequestWithoutOfficersUploadBatch,
                    Items = (records == null || !records.Any()) ? Enumerable.Empty<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO>() : records,
                    BatchItemsReport = uploadReport,
                    BatchToken = batchToken
                };

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Checks if generate request without officers upload file batch with id embedded in specified batch token has been successfully uploaded and validated
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        public APIResponse CheckIfBatchUploadValidationCompleted(string batchToken)
        {
            try
            {
                if (string.IsNullOrEmpty(batchToken))
                {
                    Logger.Error("Batch token not specified");
                    return new APIResponse { ResponseObject = "Batch token not specified", Error = true };
                }

                GenerateRequestWithoutOfficersUploadBatchStagingDTO batchDTO = _generateRequestWithoutOfficersUploadBatchStagingManager.GetGenerateRequestWithoutOfficersUploadBatchStatusInfoWithId(GetFileProcessModel(batchToken).Id);

                if (batchDTO.Status == (int)GenerateRequestWithoutOfficersUploadStatus.BatchValidated)
                {
                    return new APIResponse { ResponseObject = new { Completed = true } };
                }
                else if (batchDTO.Status == (int)GenerateRequestWithoutOfficersUploadStatus.Fail)
                {
                    return new APIResponse { ResponseObject = batchDTO.ErrorMessage, Error = true };
                }
                else if (batchDTO.HasError)
                {
                    return new APIResponse { ResponseObject = "Error occured while processing uploaded file, please upload a new file and try again", Error = true };
                }
                return new APIResponse { ResponseObject = new { Completed = false } };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { ResponseObject = CBS.Core.Lang.ErrorLang.genericexception().Text, Error = true };
            }
        }


        private FileProcessModel GetFileProcessModel(string batchToken)
        {
            return JsonConvert.DeserializeObject<FileProcessModel>(Util.LetsDecrypt(batchToken, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]));
        }


        /// <summary>
        /// Saves file and creates GenerateRequestWithoutOfficersUploadBatchStaging batch
        /// </summary>
        /// <param name="generateRequestWithoutOfficersUploadFile"></param>
        /// <param name="taxEntityProfileLocationId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string ProcessGenerateRequestWithoutOfficersUploadFile(HttpPostedFileBase generateRequestWithoutOfficersUploadFile, int taxEntityProfileLocationId, string fileName)
        {
            string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

            StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
            Node node = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.PSSEGSGenerateRequestWithoutOfficersUploadFilePath)).FirstOrDefault();
            if (string.IsNullOrEmpty(node?.Value))
            {
                throw new Exception("Unable to get pss admin generate request without officers upload file path in config file");
            }

            DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value);
            string filePath = Path.Combine(basePath.FullName, fileName);

            //create batch model
            GenerateRequestWithoutOfficersUploadBatchStaging batch = new GenerateRequestWithoutOfficersUploadBatchStaging
            {
                TaxEntityProfileLocation = new TaxEntityProfileLocation { Id = taxEntityProfileLocationId },
                Status = (int)GenerateRequestWithoutOfficersUploadStatus.BatchInitialized,
                FilePath = filePath,
                AddedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id }
            };

            //save file
            generateRequestWithoutOfficersUploadFile.SaveAs(filePath);

            //Save batch
            if (!_generateRequestWithoutOfficersUploadBatchStagingManager.Save(batch))
            {
                Logger.Error($"Unable to create GenerateRequestWithoutOfficersUploadBatchStaging batch for entity with tax entity profile location id {taxEntityProfileLocationId}");
                _generateRequestWithoutOfficersUploadBatchStagingManager.RollBackAllTransactions();
                throw new Exception($"Unable to create GenerateRequestWithoutOfficersUploadBatchStaging batch for entity with tax entity profile location id {taxEntityProfileLocationId}");
            }

            //Pass the batch id and tenant name to hangfire to extract the items from the file as a background job
            IGenerateRequestWithoutOfficersFileUpload generateRequestWithoutOfficersFileUpload = new GenerateRequestWithoutOfficersFileUpload();
            generateRequestWithoutOfficersFileUpload.ProcessGenerateRequestWithoutOfficersFileUpload(batch.Id, siteName.Replace(" ", ""));

            //Return batch token
            FileProcessModel processedBatchId = new FileProcessModel { Id = batch.Id };
            return Util.LetsEncrypt(JsonConvert.SerializeObject(processedBatchId), AppSettingsConfigurations.EncryptionSecret);
        }

        /// <summary>
        /// Get GenerateRequestDetailVM for the view Generate Request Details
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public GenerateRequestWithoutOfficersDetail GetGenerateRequestDetailVM(GenerateRequestWithoutOfficersUploadBatchItemsStagingReportSearchParams searchParams, bool isDefault)
        {
            TaxEntityProfileLocationVM taxEntityProfileLocationVM = _taxEntityProfileLocationManager.GetTaxEntityLocationWithId(searchParams.ProfileId);

            dynamic recordsAndAggregate = _generateRequestWithoutOfficersUploadBatchItemsStagingFilter.GetReportViewModel(searchParams);
            IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO> records = ((IEnumerable<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO>)recordsAndAggregate.ReportRecords);
            FileUploadReport uploadReport = ((IEnumerable<FileUploadReport>)recordsAndAggregate.ValidItemsAggregate).First();
            uploadReport.NumberOfRecords = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount;

            return new GenerateRequestWithoutOfficersDetail
            {
                BranchTaxEntityProfileLocation = taxEntityProfileLocationVM,
                OfficersRequests = (records == null || !records.Any()) ? Enumerable.Empty<GenerateRequestWithoutOfficersUploadBatchItemsStagingDTO>() : records,
                TotalRecordCount = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount,
                ProfileId = searchParams.ProfileId,
                BatchId = searchParams.GenerateRequestWithoutOfficersUploadBatchStagingId,
                GenerateRequestWithoutOfficersUploadBatchItemsReport = uploadReport,
                Status = _generateRequestWithoutOfficersUploadBatchStagingManager.GetGenerateRequestWithoutOfficersUploadBatchStatusInfoWithId(searchParams.GenerateRequestWithoutOfficersUploadBatchStagingId).Status
            };
        }
    }
}