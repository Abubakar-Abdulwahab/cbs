using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.DataFilters.TaxEntityProfileLocationReport.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Police.Core.Models.Enums;
using System.IO;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.DataFilters.PSSBranchSubUsersUpload.Contracts;
using Parkway.CBS.Police.Core.VM;
using Newtonsoft.Json;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts;
using Parkway.CBS.POSSAP.EGSRegularization.Implementations;
using Parkway.CBS.Police.Core.CoreServices.Contracts;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSBranchSubUsersUploadHandler : IPSSBranchSubUsersUploadHandler
    {
        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;
        private readonly ICBSUserManager<CBSUser> _cbsUserManager;
        private readonly ITaxEntityProfileLocationFilter _taxEntityProfileLocationFilter;
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreHelperService _corehelper;
        private readonly IPSSBranchSubUsersUploadBatchStagingManager<PSSBranchSubUsersUploadBatchStaging> _iPSSBranchSubUsersUploadBatchStagingManager;
        private readonly IPSSBranchSubUsersUploadBatchItemsStagingFilter _iPSSBranchSubUsersUploadBatchItemsStagingFilter;
        private readonly ICorePSSBranchSubUsersUploadBatchItemsStagingService _corePSSBranchSubUsersUploadBatchItemsStagingService;
        ILogger Logger { get; set; }
        public PSSBranchSubUsersUploadHandler(IOrchardServices orchardServices, ITaxEntityManager<TaxEntity> taxEntityManager, ITaxEntityProfileLocationFilter taxEntityProfileLocationFilter, IHandlerComposition handlerComposition, ICoreHelperService corehelper, IPSSBranchSubUsersUploadBatchStagingManager<PSSBranchSubUsersUploadBatchStaging> iPSSBranchSubUsersUploadBatchStagingManager, ICBSUserManager<CBSUser> cbsUserManager, IPSSBranchSubUsersUploadBatchItemsStagingFilter iPSSBranchSubUsersUploadBatchItemsStagingFilter, ICorePSSBranchSubUsersUploadBatchItemsStagingService corePSSBranchSubUsersUploadBatchItemsStagingService)
        {
            _orchardServices = orchardServices;
            _cbsUserManager = cbsUserManager;
            _handlerComposition = handlerComposition;
            _taxEntityManager = taxEntityManager;
            _taxEntityProfileLocationFilter = taxEntityProfileLocationFilter;
            _corehelper = corehelper;
            _iPSSBranchSubUsersUploadBatchStagingManager = iPSSBranchSubUsersUploadBatchStagingManager;
            _iPSSBranchSubUsersUploadBatchItemsStagingFilter = iPSSBranchSubUsersUploadBatchItemsStagingFilter;
            _corePSSBranchSubUsersUploadBatchItemsStagingService = corePSSBranchSubUsersUploadBatchItemsStagingService;
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
        /// Get PSSBranchDetailsVM
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSBranchDetailsVM GetPSSBranchDetailsVM(string payerId, TaxEntityProfileLocationReportSearchParams searchParams)
        {
            try
            {
                TaxEntityViewModel taxEntityViewModel = _taxEntityManager.GetTaxEntityDetailsWithPayerId(payerId.ToUpper());
                if (taxEntityViewModel == null) { return null; }
                searchParams.TaxEntityId = taxEntityViewModel.Id;
                dynamic recordsAndAggregate = _taxEntityProfileLocationFilter.GetReportViewModel(searchParams);
                IEnumerable<TaxEntityProfileLocationVM> records = ((IEnumerable<TaxEntityProfileLocationVM>)recordsAndAggregate.ReportRecords);

                return new PSSBranchDetailsVM
                {
                    EntityDetails = taxEntityViewModel,
                    Branches = (records == null || !records.Any()) ? Enumerable.Empty<TaxEntityProfileLocationVM>() : records,
                    TotalRecordCount = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount
                };

            } catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Validates branch sub users file size and type
        /// </summary>
        /// <param name="branchSubUserFile"></param>
        /// <param name="errors"></param>
        /// <returns>returns true if there are errors</returns>
        public bool ValidateBranchSubUserFile(HttpPostedFileBase branchSubUserFile, ref List<ErrorModel> errors)
        {
            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            Node node = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.PSSAdminBranchSubUserFileSize)).FirstOrDefault();
            int fileSizeCap = 0;
            if (string.IsNullOrEmpty(node?.Value) || !int.TryParse(node.Value, out fileSizeCap))
            {
                Logger.Error(string.Format("Unable to get pss admin branch sub user file size in config file"));
                errors.Add(new ErrorModel { ErrorMessage = "Error uploading branch sub users file.", FieldName = "branchSubUserFile" });
                return true;
            }
            filesAndFileNames.Add(new UploadedFileAndName { Upload = branchSubUserFile, UploadName = "branchSubUserFile" });
            _corehelper.CheckFileSize(filesAndFileNames, errors, fileSizeCap);

            return errors.Any();
        }

        /// <summary>
        /// Process branch sub users file, saving and returning the batch token
        /// </summary>
        /// <param name="branchSubUserFile"></param>
        /// <param name="payerId"></param>
        /// <returns></returns>
        public string ProcessBranchSubUsersFileUpload(HttpPostedFileBase branchSubUserFile, string payerId)
        {
            try
            {
                CBSUserVM cbsUser = _cbsUserManager.GetAdminCBSUserWithPayerId(payerId);
                if (cbsUser == null)
                {
                    Logger.Error($"Admin CBS user with payer id {payerId} not found");
                    throw new Exception();
                }
                string fileName = Util.StrongRandomNoSpecailCharacters() + DateTime.Now.Ticks.ToString() + payerId + cbsUser.Id + Path.GetExtension(branchSubUserFile.FileName);
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;

                StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
                Node node = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.PSSAdminBranchSubUserFilePath)).FirstOrDefault();
                if (string.IsNullOrEmpty(node?.Value))
                {
                    Logger.Error(string.Format("Unable to get pss admin branch sub user file path in config file"));
                    throw new Exception();
                }

                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value);
                string filePath = Path.Combine(basePath.FullName, fileName);

                //create batch model
                PSSBranchSubUsersUploadBatchStaging batch = new PSSBranchSubUsersUploadBatchStaging
                {
                    CBSUser = new CBSUser { Id = cbsUser.Id },
                    TaxEntity = new TaxEntity { Id = cbsUser.TaxEntity.Id },
                    Status = (int)PSSBranchSubUserUploadStatus.BatchInitialized,
                    FilePath = filePath,
                    FileName = fileName,
                    AddedBy = new Orchard.Users.Models.UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id }
                };

                //save file
                branchSubUserFile.SaveAs(filePath);

                //Save batch
                if (!_iPSSBranchSubUsersUploadBatchStagingManager.Save(batch))
                {
                    Logger.Error($"Unable to create PSSBranchSubUsersUploadBatchStaging batch for entity with payer id {payerId}");
                    _iPSSBranchSubUsersUploadBatchStagingManager.RollBackAllTransactions();
                    throw new Exception($"Unable to create PSSBranchSubUsersUploadBatchStaging batch for entity with payer id {payerId}");
                }

                //Pass the batch id and tenant name to hangfire to extract the items from the file as a background job
                IPSSBranchSubUsersFileUpload pssBranchSubUsersFileUploadService = new PSSBranchSubUsersFileUpload();
                pssBranchSubUsersFileUploadService.ProcessPSSBranchSubUsersFileUpload(batch.Id, siteName.Replace(" ", ""));

                //Return batch token
                FileProcessModel processedBranchSubUsersBatchId = new FileProcessModel { Id = batch.Id };
                return Util.LetsEncrypt(JsonConvert.SerializeObject(processedBranchSubUsersBatchId), AppSettingsConfigurations.EncryptionSecret);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets branch sub users file upload validation result
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PSSBranchSubUsersUploadValidationResultVM GetUploadValidationResult(string batchToken, PSSBranchSubUsersUploadBatchItemsSearchParams searchParams)
        {
            try
            {
                searchParams.BatchId = GetFileProcessModel(batchToken).Id;
                if (_iPSSBranchSubUsersUploadBatchStagingManager.Count(x => x.Id == searchParams.BatchId) == 0)
                {
                    Logger.Error($"Branch sub users file batch with Id {searchParams.BatchId} not found.");
                    throw new Exception($"Branch sub users file batch with Id {searchParams.BatchId} not found.");
                }

                PSSBranchSubUsersUploadBatchStagingDTO branchSubUsersUploadBatchStagingDTO = _iPSSBranchSubUsersUploadBatchStagingManager.GetBranchSubUsersBatchWithId(searchParams.BatchId);
                dynamic recordsAndAggregate = _iPSSBranchSubUsersUploadBatchItemsStagingFilter.GetReportViewModel(searchParams);
                IEnumerable<PSSBranchSubUsersUploadBatchItemsStagingDTO> records = ((IEnumerable<PSSBranchSubUsersUploadBatchItemsStagingDTO>)recordsAndAggregate.ReportRecords);
                FileUploadReport uploadReport = ((IEnumerable<FileUploadReport>)recordsAndAggregate.ValidItemsAggregate).First();
                uploadReport.NumberOfRecords = (int)((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount;
                return new PSSBranchSubUsersUploadValidationResultVM
                {
                    BatchDetails = branchSubUsersUploadBatchStagingDTO,
                    Items = (records == null || !records.Any()) ? Enumerable.Empty<PSSBranchSubUsersUploadBatchItemsStagingDTO>() : records,
                    PSSBranchSubUsersUploadBatchItemsReport = uploadReport,
                    BatchToken = batchToken
                };

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        private FileProcessModel GetFileProcessModel(string batchToken)
        {
            return JsonConvert.DeserializeObject<FileProcessModel>(Util.LetsDecrypt(batchToken, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]));
        }

        /// <summary>
        /// Checks if branch sub users file batch with id embedded in specified batch token has been successfully uploaded and validated
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

                PSSBranchSubUsersUploadBatchStagingDTO branchSubUsersUploadBatchStagingDTO = _iPSSBranchSubUsersUploadBatchStagingManager.GetBranchSubUsersBatchWithId(GetFileProcessModel(batchToken).Id);

                if (branchSubUsersUploadBatchStagingDTO.Status == (int)PSSBranchSubUserUploadStatus.BatchValidated)
                {
                    return new APIResponse { ResponseObject = new { Completed = true } };
                }
                else if(branchSubUsersUploadBatchStagingDTO.Status == (int)PSSBranchSubUserUploadStatus.Fail)
                {
                    return new APIResponse { ResponseObject = branchSubUsersUploadBatchStagingDTO.ErrorMessage, Error = true };
                }
                else if (branchSubUsersUploadBatchStagingDTO.HasError)
                {
                    return new APIResponse { ResponseObject = "Error occured while processing uploaded file, please upload a new file and try again", Error = true };
                }
                return new APIResponse { ResponseObject = new { Completed = false } };
            } catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { ResponseObject = CBS.Core.Lang.ErrorLang.genericexception().Text, Error = true };
            }
        }

        /// <summary>
        /// Creates branch sub users
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns>payer id of tax entity attached to the batch</returns>
        public string SaveBranchSubUsers(string batchToken)
        {
            try
            {
                long batchId = GetFileProcessModel(batchToken).Id;
                if(_iPSSBranchSubUsersUploadBatchStagingManager.Count(x => x.Id == batchId && !x.HasError && x.Status == (int)PSSBranchSubUserUploadStatus.BatchValidated) == 0)
                {
                    Logger.Error($"PSS Branch Sub Users Upload Batch Staging with Id {batchId} not found");
                    throw new Exception();
                }
                _corePSSBranchSubUsersUploadBatchItemsStagingService.CreateBranchSubUsers(batchId);

                //set batch status to completed
                _iPSSBranchSubUsersUploadBatchStagingManager.UpdateStatusForBatchWithId(batchId, PSSBranchSubUserUploadStatus.Completed);

                //return payer id
                return _iPSSBranchSubUsersUploadBatchStagingManager.GetPayerIdForBatchTaxEntity(batchId);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}