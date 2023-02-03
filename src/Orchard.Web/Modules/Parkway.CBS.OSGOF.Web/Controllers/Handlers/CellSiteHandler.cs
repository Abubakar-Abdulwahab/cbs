using Parkway.CBS.Module.Web.Controllers.Handlers;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.Contracts;
using Orchard;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using System;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Core.Exceptions;
using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.FileUpload.Implementations.Contracts;
using Parkway.CBS.OSGOF.Web.CoreServices.Contracts;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.BillingImpl.Contracts;
using System.Web;
using System.IO;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.CoreServices.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using Parkway.CBS.OSGOF.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using System.Linq;
using Orchard.Users.Models;
using Parkway.CBS.Entities.VMs;

namespace Parkway.CBS.OSGOF.Web.Controllers.Handlers
{
    public class CellSiteHandler : CommonBaseHandler, ICellSiteHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICoreOSGOFService _coreOSGOFService;
        //private readonly IEnumerable<IMDAFilter> _dataFilters;

        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly IFormControlRevenueHeadManager<FormControlRevenueHead> _formRevenueHeadRepository;
        private readonly IFormControlsManager<FormControl> _formcontrolsRepository;
        private readonly IEnumerable<IOSGOFBillingImpl> _billingImpls;

        private readonly ICoreUserService _coreUserService;
        private readonly IFileUploadConfiguration _fileUploadConfig;
        private readonly ICoreCellSites _coreCellSitesAdmin;
        private readonly ICellSitesHandler _cellSiteHandler;
        private readonly ICellSiteManager<CellSites> _cellSitesService;
        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;


        public CellSiteHandler(IOrchardServices orchardServices, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreOSGOFService coreCollectionService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, IFormControlsManager<FormControl> formcontrolsRepository, IEnumerable<IOSGOFBillingImpl> billingImpls, ICoreCellSites coreCellSitesAdmin, ICellSitesHandler cellSiteHandler, ICellSiteManager<CellSites> cellSitesService, ITaxEntityManager<TaxEntity> taxEntityManager) : base(orchardServices, settingsRepository, handlerHelper)
        {
            _coreOSGOFService = coreCollectionService;
            _taxCategoriesRepository = taxCategoriesRepository;
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
            _coreUserService = coreUserService;
            _formRevenueHeadRepository = formRevenueHeadRepository;
            _formcontrolsRepository = formcontrolsRepository;
            _billingImpls = billingImpls;
            _fileUploadConfig = new FileUploadConfiguration();
            _coreCellSitesAdmin = coreCellSitesAdmin;
            _cellSiteHandler = cellSiteHandler;
            _cellSitesService = cellSitesService;
            _taxEntityManager = taxEntityManager;
        }


        /// <summary>
        /// Process file upload assessment for OSGOF
        /// </summary>
        /// <param name="file"></param>
        /// <param name="processStage"></param>
        /// <param name="entity"></param>
        /// <param name="cBSUser"></param>
        /// <returns>ProcessingReportVM</returns>
        public virtual ProcessingReportVM ProcessCellSitesFileUpload(HttpPostedFileBase file, GenerateInvoiceStepsModel processStage, TaxEntity entity, CBSUser authorizedUser)
        {
            try
            {
                Logger.Information("Processing on sceeen handler");
                //get the category
                Logger.Information("Getting category");
                TaxEntityCategory category = _coreOSGOFService.GetTaxEntityCategory(processStage.CategoryId);
                //check if the category requires login
                if (category.RequiresLogin) { if (authorizedUser == null) { throw new AuthorizedUserNotFoundException(); } }

                //if all clear lets get the revenue head
                Logger.Information("calling db revenue head.");
                RevenueHeadDetails revenueHeadDetails = _coreOSGOFService.GetRevenueHeadDetails(processStage.RevenueHeadId);
                if (revenueHeadDetails.Mda == null) { throw new MDARecordNotFoundException(); }

                if (revenueHeadDetails.Billing == null) { throw new NoBillingInformationFoundException(); }
                Logger.Information("Gotten billing and mda record");

                //now we get the tenplate and the implementing interface
                var fileUploadModelJson = JsonConvert.DeserializeObject<dynamic>(revenueHeadDetails.Billing.FileUploadModel);
                var selectedTemplate = fileUploadModelJson.SelectedTemplate.Value as string;
                //lets save the batch record
                CellSiteClientPaymentBatch batch = new CellSiteClientPaymentBatch
                {
                    RevenueHead = revenueHeadDetails.RevenueHead,
                    Type = Core.Models.Enums.PayeAssessmentType.FileUpload,
                    Origin = "CentralBillingSystem",
                    TaxEntity = entity,
                    Template = revenueHeadDetails.Billing.FileUploadModel,
                    CBSUser = authorizedUser,
                };

                _coreOSGOFService.SaveCellSiteClientPaymentBatch(batch);

                FileProcessModel objValue = new FileProcessModel { Id = batch.Id };
                string token = Util.LetsEncrypt(JsonConvert.SerializeObject(objValue), AppSettingsConfigurations.EncryptionSecret);

                //con file name
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                string fileName = string.Format("batchId-{0}-_-siteName-{1}.xls", batch.Id, siteName);
                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "/App_data/Media/CellSites/" + siteName);
                string path = Path.Combine(basePath.FullName, fileName);
                batch.FileName = fileName;
                batch.FilePath = path;

                file.SaveAs(path);
                return new ProcessingReportVM { AdapterValue = selectedTemplate, RequestToken = token };
            }
            catch (Exception exception)
            {
                _settingsRepository.RollBackAllTransactions();
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public virtual ProcessingReportVM ProcessOSGOFOnScreenAssessment(ICollection<FileUploadCellSites> cellSites, GenerateInvoiceStepsModel processStage, TaxEntity entity, CBSUser authorizedUser)
        {
            Logger.Information("Processing on sceen handler");
            //get the category
            Logger.Information("Getting category");
            TaxEntityCategory category = _coreOSGOFService.GetTaxEntityCategory(processStage.CategoryId);
            //check if the category requires login
            if (category.RequiresLogin) { if (authorizedUser == null) { throw new AuthorizedUserNotFoundException(); } }

            //if all clear lets get the revenue head
            Logger.Information("calling db revenue head.");
            RevenueHeadDetails revenueHeadDetails = _coreOSGOFService.GetRevenueHeadDetails(processStage.RevenueHeadId);
            if (revenueHeadDetails.Mda == null) { throw new MDARecordNotFoundException(); }

            if (revenueHeadDetails.Billing == null) { throw new NoBillingInformationFoundException(); }
            Logger.Information("Gotten billing and mda record");

            //now we get the tenplate and the implementing interface
            var fileUploadModelJson = JsonConvert.DeserializeObject<dynamic>(revenueHeadDetails.Billing.FileUploadModel);

            var selectedTemplate = fileUploadModelJson.SelectedTemplate.Value as string;
            var selectedImpl = fileUploadModelJson.SelectedImplementation.Value as string;

            //lets get the FileUploadTemplate
            IFileUploadImplementations implementingClass = _fileUploadConfig.GetImplementation(selectedTemplate, selectedImpl, Util.GetAppXMLFilePath());

            CellSitesBreakDown response = implementingClass.ProcessOnScreenCellSitesForOSGOF(Util.GetOSGOFXMLFilePath(), cellSites);
            //lets save the batch record

            CellSiteClientPaymentBatch batch = new CellSiteClientPaymentBatch
            {
                RevenueHead = revenueHeadDetails.RevenueHead,
                Type = Core.Models.Enums.PayeAssessmentType.OnScreen,
                Origin = "CentralBillingSystem",
                PercentageProgress = 100,
                TaxEntity = entity,
                Template = JsonConvert.SerializeObject(new { SelectedTemplate = selectedTemplate, SelectedImplementation = selectedImpl }),
                CBSUser = authorizedUser,
                TotalNoOfRowsProcessed = response.CellSiteModelV2.Count,
            };

            _coreOSGOFService.SaveCellSiteClientPaymentBatch(batch);

            FileProcessModel objValue = new FileProcessModel { Id = batch.Id };
            string token = Util.LetsEncrypt(JsonConvert.SerializeObject(objValue), AppSettingsConfigurations.EncryptionSecret);

            try
            {
                _coreOSGOFService.SaveCellSitesForOnScreenInput(batch, response, entity);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _settingsRepository.RollBackAllTransactions();
                throw;
            }

            return new ProcessingReportVM { AdapterValue = selectedTemplate, RequestToken = token };
        }


        /// <summary>
        /// Process cell sites file upload
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns>APIResponse</returns>
        public APIResponse ProcessCellSiteFile(string batchToken)
        {
            try
            {
                Logger.Information("Decrypting osgof cell site file processing batch token");
                var decryptedValue = Util.LetsDecrypt(batchToken, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]);
                var objValue = JsonConvert.DeserializeObject<FileProcessModel>(decryptedValue);
                return _coreOSGOFService.ProcessCellSiteFileUpload(objValue);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " .Error decrypting batch token " + batchToken);
            }

            return new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        public APIResponse ValidateCellSitesAgainstStoredRecords(string batchToken)
        {
            try
            {
                Logger.Information("Decrypting osgof cell site file processing batch token");
                var decryptedValue = Util.LetsDecrypt(batchToken, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]);
                var objValue = JsonConvert.DeserializeObject<FileProcessModel>(decryptedValue);
                return _coreOSGOFService.DoCellSitesComparison(objValue);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " .Error decrypting batch token " + batchToken);
            }
            return new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true };
        }


        /// <summary>
        /// Once the file contents have been read and validated, this method would get the payment details to construct the view
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPaymentScheduleDetails(string batchToken)
        {
            try
            {
                Logger.Information("Decrypting osgof cell site file processing batch token");
                var decryptedValue = Util.LetsDecrypt(batchToken, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]);
                var objValue = JsonConvert.DeserializeObject<FileProcessModel>(decryptedValue);
                return _coreOSGOFService.GetScheduleDetails(objValue);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " .Error decrypting batch token " + batchToken);
            }
            return new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true };
        }


        /// <summary>
        /// Get the next page for cell site report
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="page"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPagedCellSiteData(string batchToken, int page)
        {
            try
            {
                Logger.Information("Decrypting osgof cell site file processing batch token");
                var decryptedValue = Util.LetsDecrypt(batchToken, System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]);
                var objValue = JsonConvert.DeserializeObject<FileProcessModel>(decryptedValue);
                int take = 10;
                int skip = page == 1 ? 0 : (take * page) - take;
                var result = _coreOSGOFService.GetCellSites(objValue, take, skip);
                return new APIResponse { ResponseObject = result };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " .Error decrypting batch token " + batchToken);
            }
            return new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true };
        }


        /// <summary>
        /// Get the next paged data for schedule upload for client
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <param name="page"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPagedCellSiteForScheduleUpload(string scheduleRef, int page)
        {
            try
            {
                Logger.Information("Decrypting osgof cell site file processing batch token");

                int take = 10;
                int skip = page == 1 ? 0 : (take * page) - take;
                CellSitesStagingReportVM record = _coreCellSitesAdmin.GetCellSitesStaging(scheduleRef, 10, skip);
                return new APIResponse { ResponseObject = record };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " .Error getting data for scheduleRef " + scheduleRef);
            }
            return new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true };
        }


        public CellSitesVM GetCellSites(long operatorId, int page, int pageSize)
        {
            List<CellSitesDetailsVM> cellSites = new List<CellSitesDetailsVM>();
            try
            {
                var searchRecords = _cellSitesService.GetCellSites(operatorId, page, pageSize);
                if (searchRecords.Count() > 0)
                {
                    cellSites = searchRecords.Select(x => new CellSitesDetailsVM
                    {
                        Id = x.Id,
                        OperatorSiteId = x.OperatorSiteId,
                        YearOfDeployment = x.YearOfDeployment,
                        HeightOfTower = x.HeightOfTower,
                        Long = x.Long,
                        Lat = x.Lat,
                        SiteAddress = x.SiteAddress,
                        OSGOFID = x.OSGOFId,
                        State = x.State.Name,
                        LGA = x.LGA.Name,
                        //Operator = new TaxEntityProfileVM { Address = x.TaxProfile.Address,  } x.TaxProfile,
                    }).ToList();
                }

                var result = _cellSitesService.GetAggregateForCellSites(operatorId).First();

                return new CellSitesVM
                {
                    CellSites = cellSites,
                    TotalNumberOfCellSites = result.TotalNumberOfCellSites
                };
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error getting cell sites list", exception.Message), exception);
                throw;
            }
        }



        /// <summary>
        /// Get the next paged data for cell sites for operator page display
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="page"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPagedCellSiteList(long operatorId, int page)
        {
            try
            {
                int take = 10;
                int skip = page == 1 ? 0 : (take * page) - take;
                CellSitesVM record = _cellSiteHandler.GetCellSites(operatorId, skip, take);
                return new APIResponse { ResponseObject = record };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " .Error getting data for scheduleRef " + operatorId);
            }
            return new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true };
        }


        /// <summary>
        /// Get data from cell sites schedule client side
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns>CellSitesStagingReportVM</returns>
        public CellSitesStagingReportVM GetStagingDataForSchdedule(string scheduleRef, int take, int skip)
        {
            return _cellSiteHandler.GetScheduleStagingData(scheduleRef, take, skip);
        }


        /// <summary>
        /// Complete cell sites processing for client side cell sites upload
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <returns>CellSitesStagingReportVM</returns>
        public CellSitesStagingReportVM CompleteCellSitesProcessingForClientUpload(string scheduleRef)
        {
            return _cellSiteHandler.CompleteCellSiteProcessing(scheduleRef);
        }



        /// <summary>
        /// Process cell sites file
        /// </summary>
        /// <param name="taxProfile"></param>
        /// <param name="file"></param>
        /// <returns>CellSitesFileValidationObject</returns>
        public CellSitesFileValidationObject CreateCellSites(string payerId, HttpPostedFileBase file, UserPartRecord adminUser, CBSUser loggedInUser)
        {
            try
            {
                //con file name
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                string fileName = string.Format("payerId-{1}-{3}-siteName-{0}-{2}", siteName, payerId, file.FileName, DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss"));
                DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + "/App_data/Media/CellSites/" + siteName);
                string path = Path.Combine(basePath.FullName, fileName);
                file.SaveAs(path);
                //
                var taxProfileDetails = _taxEntityManager.GetTaxPayerWithDetails(payerId);
                if (taxProfileDetails == null) { throw new NoRecordFoundException("Could not find tax payer Id"); }

                return _coreCellSitesAdmin.StoreCellSitesIntoStaging(new TaxEntity { Id = taxProfileDetails.Id, TaxEntityCategory = new TaxEntityCategory { Id = taxProfileDetails.CategoryId } }, adminUser, loggedInUser, path);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw;
            }
        }

    }
}