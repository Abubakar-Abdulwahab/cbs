using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using System.IO;
using Parkway.CBS.TaxPayerEnumerationService.Implementations.Contracts;
using Parkway.CBS.TaxPayerEnumerationService.Implementations;
using Parkway.CBS.ClientFileServices.Implementations.TaxPayerEnumeration;
using Parkway.CBS.ClientFileServices.Implementations.Contracts;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class TaxPayerEnumerationHandler : ITaxPayerEnumerationHandler
    {
        private readonly ICoreTaxPayerEnumerationService _coreTaxPayerEnumerationService;
        private readonly ICoreStateAndLGA _coreStateAndLgaService;
        private readonly ITaxPayerEnumerationValidation _taxPayerEnumerationValidator;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }
        public TaxPayerEnumerationHandler(ICoreTaxPayerEnumerationService coreTaxPayerEnumerationService, ICoreStateAndLGA coreStateAndLgaService, IOrchardServices orchardServices)
        {
            _coreTaxPayerEnumerationService = coreTaxPayerEnumerationService;
            _coreStateAndLgaService = coreStateAndLgaService;
            _taxPayerEnumerationValidator = new TaxPayerEnumerationValidation();
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Gets States and LGAs
        /// </summary>
        /// <returns></returns>
        public List<StateModel> GetStatesAndLgas()
        {
            try
            {
                return _coreStateAndLgaService.GetStates();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Process enumeration line items from on screen form.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="userModel"></param>
        /// <returns>BatchToken</returns>
        public string ProcessEnumerationItemsForOnScreenForm(ICollection<TaxPayerEnumerationLine> items, UserDetailsModel userModel)
        {
            try
            {
                TaxPayerEnumeration enumerationBatchRecord = new TaxPayerEnumeration
                {
                    Employer = new TaxEntity { Id = userModel.TaxPayerProfileVM.Id },
                    UploadType = (int)EnumerationScheduleUploadType.OnScreen,
                    UploadTypeCode = EnumerationScheduleUploadType.OnScreen.ToDescription(),
                    UploadedByAdmin = false,
                    UploadedByUser = true,
                    IsActive = true,
                    User = userModel.CBSUser,
                    ProcessingStage = (int)TaxPayerEnumerationProcessingStages.Completed,
                };

                _coreTaxPayerEnumerationService.ProcessItemsForOnScreen(_taxPayerEnumerationValidator.GetValidatedEnumerationItems(items.ToList()), enumerationBatchRecord);

                FileProcessModel processedEnumerationBatchId = new FileProcessModel { Id = enumerationBatchRecord.Id };

                return Util.LetsEncrypt(JsonConvert.SerializeObject(processedEnumerationBatchId), AppSettingsConfigurations.EncryptionSecret);
                
            }catch(Exception exception)
            {
                Logger.Error(exception, $"Exception occured while processing enumeration line items for on screen form in handler. Exception message -- {exception.Message}");
                throw;
            }
        }


        /// <summary>
        /// Process enumeration line items for file upload.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public string ProcessEnumerationItemsForFileUpload(HttpPostedFileBase file, UserDetailsModel userModel)
        {
            try
            {
                
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                string fileName = Guid.NewGuid().ToString("N") + DateTime.Now.Ticks.ToString() + userModel.TaxPayerProfileVM.Id + Path.GetExtension(file.FileName);

                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "/App_data/Media/TaxPayerEnumerationSchedules/" + siteName);
                string path = Path.Combine(basePath.FullName, fileName);

                TaxPayerEnumeration enumerationBatchRecord = new TaxPayerEnumeration
                {
                    Employer = new TaxEntity { Id = userModel.TaxPayerProfileVM.Id },
                    UploadType = (int)EnumerationScheduleUploadType.FileUpload,
                    UploadTypeCode = EnumerationScheduleUploadType.FileUpload.ToDescription(),
                    UploadedByAdmin = false,
                    UploadedByUser = true,
                    IsActive = false,
                    User = userModel.CBSUser,
                    ProcessingStage = (int)TaxPayerEnumerationProcessingStages.BatchInitialized,
                    FilePath = path,
                    FileName = fileName
                };

                //save file
                file.SaveAs(path);

                //Save enumeration batch
                _coreTaxPayerEnumerationService.SaveEnumerationBatchRecord(enumerationBatchRecord);

                //Pass the request to hangfire to execute as a background job
                ITaxPayerEnumerationFileUpload enumerationFileUploadService = new TaxPayerEnumerationFileUpload();
                enumerationFileUploadService.ProcessTaxPayerEnumerationFileUpload(enumerationBatchRecord.Id, enumerationBatchRecord.FilePath, siteName.Replace(" ", ""));

                FileProcessModel processedEnumerationBatchId = new FileProcessModel { Id = enumerationBatchRecord.Id };

                //Return batch token
                return Util.LetsEncrypt(JsonConvert.SerializeObject(processedEnumerationBatchId), AppSettingsConfigurations.EncryptionSecret);

            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Exception occured while processing enumeration line items for file upload in handler. Exception message -- {exception.Message}");
                throw;
            }
        }


        private FileProcessModel GetFileProcessModel(string batchToken)
        {
            return JsonConvert.DeserializeObject<FileProcessModel>(Util.LetsDecrypt(batchToken, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]));
        }


        /// <summary>
        /// Checks for the completion status of the enumeration batch line items upload.
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public APIResponse CheckIfEnumerationUploadIsCompleted(string batchToken, long taxEntityId)
        {
            try
            {
                TaxPayerEnumerationProcessingStages currentStage = (TaxPayerEnumerationProcessingStages)_coreTaxPayerEnumerationService.CheckForEnumerationScheduleUploadCompletionStatus(GetFileProcessModel(batchToken).Id, taxEntityId);
                if(currentStage == TaxPayerEnumerationProcessingStages.Completed)
                {
                    return new APIResponse { ResponseObject = true };
                }else if(currentStage == TaxPayerEnumerationProcessingStages.Fail)
                {
                    return new APIResponse { Error = true, ResponseObject = TaxPayerEnumerationProcessingStages.Fail.ToDescription() };
                }
                else
                {
                    return new APIResponse { ResponseObject = false };
                }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get enumeration line items for enumeration batch with id embedded in batch token
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public APIResponse GetLineItemsForEnumerationWithId(string batchToken, long taxEntityId)
        {
            try
            {
                Logger.Information("Decrypting tax payer enumeration schedule file processing batch token");
                FileProcessModel objValue = GetFileProcessModel(batchToken);
                //get the report details
                return new APIResponse { ResponseObject = _coreTaxPayerEnumerationService.GetReportDetails(objValue.Id, taxEntityId) };
            }
            catch(NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.norecord404().Text };
            }
            catch(Exception exception)
            {
                Logger.Error(exception, $"Exception when trying to retrieve enumeration line items. Exception message -- {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get paged line items for enumeration batch with specified id embedded in provided batch token.
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="taxEntityId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public APIResponse GetPagedLineItemsForEnumerationWithId(string batchToken, long taxEntityId, int page)
        {
            try
            {
                Logger.Information("Decrypting tax payer enumeration schedule file processing batch token");
                FileProcessModel objValue = GetFileProcessModel(batchToken);
                //get the report details
                int take = 10;
                int skip = page == 1 ? 0 : (take * page) - take;
                return new APIResponse { ResponseObject = _coreTaxPayerEnumerationService.GetPagedEnumerationLineItems(objValue.Id, taxEntityId, skip, take) };
            }
            catch(Exception exception)
            {
                Logger.Error(exception, $"Exception getting paged enumeration line items. Exception -- {exception.Message}");
                throw;
            }
        }
    }
}