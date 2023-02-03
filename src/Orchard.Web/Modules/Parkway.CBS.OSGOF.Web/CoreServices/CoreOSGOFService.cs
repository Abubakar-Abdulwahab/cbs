using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.HTTP.Handlers;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Web.CoreServices.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.FileUpload;
using System.Collections.Generic;
using System.Linq;
using System;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.Lang;
using Newtonsoft.Json;
using Parkway.CBS.FileUpload.Implementations.Contracts;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;

namespace Parkway.CBS.OSGOF.Web.CoreServices
{
    public class CoreOSGOFService : CoreBaseService, ICoreOSGOFService
    {
        private readonly ICellSiteClientPaymentBatchManager<CellSiteClientPaymentBatch> _cellSitesPaymentBatchRepo;
        private readonly ICellSitesPaymentManager<CellSitesPayment> _cellSitesPaymentRepo;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly IRevenueHeadManager<RevenueHead> _revenueHeadRepository;
        private readonly IFileUploadConfiguration _fileUploadConfig;


        public CoreOSGOFService(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, ICellSiteClientPaymentBatchManager<CellSiteClientPaymentBatch> cellSitesPaymentBatchRepo, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, IRevenueHeadManager<RevenueHead> revenueHeadRepository, ICellSitesPaymentManager<CellSitesPayment> cellSitesPaymentRepo) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            Logger = NullLogger.Instance;
            _cellSitesPaymentBatchRepo = cellSitesPaymentBatchRepo;
            _taxCategoriesRepository = taxCategoriesRepository;
            _revenueHeadRepository = revenueHeadRepository;
            _cellSitesPaymentRepo = cellSitesPaymentRepo;
            _fileUploadConfig = new FileUploadConfiguration();
        }


        /// <summary>
        /// Get some details about this revenue head
        /// <para>Gets you the revenue head, mda, and billing</para>
        /// </summary>
        /// <param name="revenueHeadId">Revenue head Id</param>
        /// <returns>RevenueHeadDetails</returns>
        public RevenueHeadDetails GetRevenueHeadDetails(int revenueHeadId)
        {
            var revenueHead = _revenueHeadRepository.GetRevenueHeadDetails(revenueHeadId);
            if (revenueHead == null) { throw new CannotFindRevenueHeadException(); }
            return revenueHead;
        }

        /// <summary>
        /// Get tax category
        /// </summary>
        /// <param name="categoryType"></param>
        /// <returns>TaxEntityCategory</returns>
        /// <exception cref="NoCategoryFoundException"></exception>
        public TaxEntityCategory GetTaxEntityCategory(int catId)
        {
            Logger.Information("Category string parse. calling db");
            var category = _taxCategoriesRepository.Get(catId);
            if (category == null) { throw new NoCategoryFoundException("No category found for the category name " + catId); }
            return category;
        }

        /// <summary>
        /// Save OSGOF Batch Record
        /// </summary>
        /// <param name="batch">SaveCellSiteClientPaymentBatch</param>
        public void SaveCellSiteClientPaymentBatch(CellSiteClientPaymentBatch batch)
        {
            if (!_cellSitesPaymentBatchRepo.Save(batch)) { throw new CouldNotSaveRecord("Could not save OSGOF batch record"); }
        }

        /// <summary>
        /// Save the cellsites for onscreen input
        /// </summary>
        /// <param name="record"></param>
        /// <param name="cellSites"></param>
        /// <param name="entity"></param>
        public void SaveCellSitesForOnScreenInput(CellSiteClientPaymentBatch record, CellSitesBreakDown cellSitesObj, TaxEntity entity)
        {
            _cellSitesPaymentRepo.SaveCellSites(record.Id, cellSitesObj);
        }


        /// <summary>
        /// Get batch record
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns>CellSiteClientPaymentBatch</returns>
        public CellSiteClientPaymentBatch GetBatchRecord(long recordId)
        {
            return _cellSitesPaymentBatchRepo.GetRecord(recordId);
        }


        /// <summary>
        /// Get the cell site report
        /// </summary>
        /// <param name="record"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="entity"></param>
        /// <returns>CellSiteReportVM</returns>
        public CellSiteReportVM GetCellSitesReport(CellSiteClientPaymentBatch record, int take, int skip, TaxEntity entity, bool getStats = false)
        {
            CellSiteReportQueryObj result = _cellSitesPaymentRepo.GetRecords(record, take, skip, getStats);

            int count = record.TotalNoOfRowsProcessed;

            int chunkSize = take;
            var dataSize = count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }

            if (entity == null)
            { entity = record.TaxEntity; }

            throw new Exception { };
            return new CellSiteReportVM
            {
                //Amount = string.Format("{0:n}", record.Amount),
                PageSize = pages,
                CellSites = result.CellSites.ToList(),
                Recipient = entity.Recipient,
                TIN = entity.TaxPayerIdentificationNumber,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                CellSitesReport = new FileUploadReport
                {
                    
                }
            };
        }

        /// <summary>
        /// Clear nhibernate session from memory
        /// </summary>
        public void ClearSession() { _cellSitesPaymentBatchRepo.ClearSession(); }


        /// <summary>
        /// Process cell site file upload 
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns>APIResponse</returns>
        public APIResponse ProcessCellSiteFileUpload(FileProcessModel objValue)
        {
            CellSiteClientPaymentBatch record = _cellSitesPaymentBatchRepo.Get(x => x.Id == objValue.Id);
            try
            {
                _cellSitesPaymentRepo.Delete(record);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true };
            }
            var fileUploadModelJson = JsonConvert.DeserializeObject<dynamic>(record.Template);

            var selectedTemplate = fileUploadModelJson.SelectedTemplate.Value as string;
            var selectedImpl = fileUploadModelJson.SelectedImplementation.Value as string;

            //lets get the FileUploadTemplate
            CellSiteFileProcessResponse fileContents = _fileUploadConfig.ReadFile(record.FilePath);
            if (fileContents.HeaderValidationObject.Error)
            {
                return new APIResponse { Error = true, ResponseObject = fileContents.HeaderValidationObject.ErrorMessage };
            }

            IFileUploadImplementations implementingClass = _fileUploadConfig.GetImplementation(selectedTemplate, selectedImpl, Util.GetAppXMLFilePath());
            //implementingCla
            CellSitesBreakDown response = implementingClass.ProcessFileUploadCellSitesForOSGOF(Util.GetOSGOFXMLFilePath(), fileContents);
            //save the responses
            _cellSitesPaymentRepo.SaveCellSites(record.Id, response);
            record.TotalNoOfRowsProcessed = response.CellSiteModelV2.Count();
            record.PercentageProgress = 100m;
            return new APIResponse { };
        }


        /// <summary>
        /// Do a cell sites comparison between the cell sites added by the user against the ones on the system
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns>APIResponse</returns>
        public APIResponse DoCellSitesComparison(FileProcessModel objValue)
        {
            CellSiteClientPaymentBatch record = _cellSitesPaymentBatchRepo.Get(x => x.Id == objValue.Id);
            if(record == null) { throw new NoRecordFoundException("Could not find cell sites batch record with Id" + objValue.Id); }
            _cellSitesPaymentRepo.RunComparisonForCellSites(record.Id);            
            return new APIResponse { ResponseObject = true  };
        }


        /// <summary>
        /// Get the details for the 
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public APIResponse GetScheduleDetails(FileProcessModel objValue)
        {
            CellSiteClientPaymentBatch record = _cellSitesPaymentBatchRepo.Get(x => x.Id == objValue.Id);
            if (record == null) { throw new NoRecordFoundException("Could not find cell sites batch record with Id" + objValue.Id); }

            int count = record.TotalNoOfRowsProcessed;

            int chunkSize = 10;
            var dataSize = count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            CellSiteReportQueryObj reportModel = _cellSitesPaymentRepo.GetRecords(record, 10, 0, true);
            var numberOfInValidRecords = record.TotalNoOfRowsProcessed - reportModel.RecordsWithoutErrors;
            //var numberOfValidRecords =  reportModel.RecordsWithErrors;
            throw new Exception { };
            var vm = new CellSiteReportVM
            {
                Amount = string.Format("{0:n}", reportModel.TotalAmount),
                PageSize = pages,
                CellSites = reportModel.CellSites.ToList(),
                CellSitesReport = new FileUploadReport
                {
                   
                }
            };
            return new APIResponse { ResponseObject = vm };
        }


        /// <summary>
        /// Get the total amount due on the given schedule
        /// </summary>
        /// <param name="record"></param>
        /// <returns>decimal</returns>
        public decimal GetTotalAmountForBatch(CellSiteClientPaymentBatch record)
        {
            return _cellSitesPaymentRepo.GetTotalAmountForSchedule(record);
        }

        /// <summary>
        /// Get list of cell sites
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name=""></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>List{CellSiteReturnModelVM}</returns>
        public List<CellSiteReturnModelVM> GetCellSites(FileProcessModel objValue, int take, int skip)
        {
            CellSiteClientPaymentBatch record = _cellSitesPaymentBatchRepo.Get(x => x.Id == objValue.Id);

            CellSiteReportQueryObj result = _cellSitesPaymentRepo.GetRecords(record, take, skip);

            return result.CellSites.ToList();
        }
    }
}