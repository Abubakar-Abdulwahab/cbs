using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Entities.DTO;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.FileUpload.OSGOFImplementation;
using Parkway.CBS.FileUpload.OSGOFImplementation.Contracts;
using Parkway.CBS.FileUpload.OSGOFImplementation.Models;
using Parkway.CBS.OSGOF.Admin.CoreServices.Contracts;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.OSGOF.Admin.CoreServices
{
    public class CoreCellSites : ICoreCellSites
    {
        private readonly ICellSiteStagingManager<CellSitesStaging> _cellSitesStagingService;
        private readonly ICellSiteScheduleStagingManager<CellSitesScheduleStaging> _cellSiteScheduleStagingService;
        private readonly ITransactionManager _transactionManager;

        private readonly IStateModelManager<StateModel> _stateRepo;
        public ILogger Logger { get; set; }
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;

        private IOSGOFCellSitesAdapter _cellSitesAdapter;
        private readonly IOperatorCategoryManager<OperatorCategory> _operatorCategoryRepo;

        public CoreCellSites(IOrchardServices orchardService, ICellSiteStagingManager<CellSitesStaging> cellSitesStagingService, IStateModelManager<StateModel> stateRepo, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICellSiteScheduleStagingManager<CellSitesScheduleStaging> cellSiteScheduleStagingService, IOperatorCategoryManager<OperatorCategory> operatorCategoryRepo)
        {
            _cellSitesStagingService = cellSitesStagingService;
            _stateRepo = stateRepo;
            Logger = NullLogger.Instance;
            _taxCategoriesRepository = taxCategoriesRepository;
            _cellSiteScheduleStagingService = cellSiteScheduleStagingService;
            _transactionManager = orchardService.TransactionManager;
            _operatorCategoryRepo = operatorCategoryRepo;
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


        public CellSitesFileValidationObject StoreCellSitesIntoStaging(TaxEntity taxProfile, UserPartRecord adminUser, CBSUser loggedInUser, string path)
        {
            _cellSitesAdapter = new OSGOFCellSitesAdapter();

            List<StatesAndLGAs> statesAndLGAs = GetStatesAndLGAs();

            OSGOFCellSitesResponse validateTemplate = _cellSitesAdapter.GetCellSitesResponseModels(statesAndLGAs, path);
            
            if (validateTemplate.HeaderValidateObject.Error)
            {
                return new CellSitesFileValidationObject { HeaderHasErrors = true, HeaderErrorMessage = validateTemplate.HeaderValidateObject.ErrorMessage };
            }
            //var cellSitesFromFile = validateTemplate.CellSites;//.OrderBy(s => s.SN.Value);
            //save schedule
            var operatorCategories = _operatorCategoryRepo.GetCollection(x => x.TaxProfileCategory == taxProfile.TaxEntityCategory);
            if(operatorCategories.Count() != 1)
            { throw new NoRecordFoundException("Could not find a unique value for the tax category in the operator's table"); }
            var operatorCategory = operatorCategories.ElementAt(0);

            CellSitesScheduleStaging scheduleStaging = SaveSchedule(validateTemplate.CellSites.Count(), taxProfile, adminUser, loggedInUser, operatorCategory, path);
            //save cell sites
            _cellSitesStagingService.SaveRecords(validateTemplate.CellSites, taxProfile, adminUser, loggedInUser, new CellSitesScheduleStaging { Id = scheduleStaging.Id });
            return new CellSitesFileValidationObject { ScheduleStagingBatchNumber = scheduleStaging.BatchRef };
        }        


        /// <summary>
        /// Get cell sites for staging
        /// </summary>
        /// <param name="scheduleBatchRef"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public CellSitesStagingReportVM GetCellSitesStaging(string scheduleBatchRef, int take, int skip)
        {
            CellSitesScheduleVM schedule = _cellSiteScheduleStagingService.GetScheduleDetails(scheduleBatchRef);
            if(schedule == null) { return new CellSitesStagingReportVM { Error = true, ErrorMessage = "No schedule found" }; }
            if (schedule.ScheduleHasAlreadyBeenTreated)
            {
                return new CellSitesStagingReportVM { Error = true, ErrorMessage = "This schedule has already been processed and approved. Please contact Parkway admin." };
            }
            //get the cell sites
            //IEnumerable<CellSitesStaging> cellSites = _cellSitesStagingService.GetCellSitesForScheduleStaging(schedule.Id, take, skip);
            var cellSites = _cellSitesStagingService.GetCellSitesForScheduleStaging(schedule.Id, take, skip)
                .Select(c => new CellSitesStagingVM
                {
                    ErrorMessages = c.ErrorMessages,
                    HasErrors = c.HasErrors,
                    HeightOfTower = c.HeightOfTowerFileValue,
                    Lat = c.Lat,
                    Long = c.Long,
                    LGA = c.LGAFileValue,
                    State = c.StateFileValue,
                    MastType = c.MastType,
                    OperatorSiteId = c.OperatorSiteId,
                    Region = c.Region,
                    SiteAddress = c.SiteAddress,
                    SNOnFile = c.SNOnFileFileValue,
                    YearOfDeployment = c.YearOfDeploymentFileValue
                });

            //CellSitesStagingVM
            //lets get the aggregate
            var aggrObj = _cellSitesStagingService.GetCellSitesWithErrors(schedule.Id);
            return new CellSitesStagingReportVM
            {
                TotalNumberOfRecords = schedule.TotalNoOfRowsProcessed,
                TotalNumberOfInvalid = aggrObj.Value,
                TotalNumberOfValidRecords = schedule.TotalNoOfRowsProcessed - aggrObj.Value,
                CellSites = cellSites.ToList(),
                PayerId = schedule.PayerId
            };
        }



        /// <summary>
        /// Move the valid cell sites from staging to the main table
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <returns></returns>
        public CellSitesStagingReportVM MoveCellSitesFromStagingToMainTable(string scheduleRef, UserPartRecord approvedBy)
        {
            //get schedule
            CellSitesScheduleStagingVM schedule = _cellSiteScheduleStagingService.GetScheduleStatgingDetails(scheduleRef);
            if (schedule == null) { return new CellSitesStagingReportVM { Error = true, ErrorMessage = "No schedule found" }; }
            if (schedule.ScheduleHasAlreadyBeenTreated)
            {
                return new CellSitesStagingReportVM { Error = true, ErrorMessage = "This schedule has already been processed and approved. Please contact Parkway admin." };
            }
            var prefixVal = Util.ZeroPadUp(schedule.ProfileId.ToString(), 2);
            schedule.OSGOFSiteIdPrefix += prefixVal;
            //do transfer
            _cellSitesStagingService.DoTransferFromStagingToMainTable(schedule, approvedBy);

            return new CellSitesStagingReportVM { Message = string.Format("Cell sites for schedule {0} has been approved and added to the list of cell sites for Operator {1} with Payer Id {2}", scheduleRef, schedule.Name, schedule.PayerId) };
        }


        private CellSitesScheduleStaging SaveSchedule(int count, TaxEntity taxProfile, UserPartRecord adminUser, CBSUser loggedInUser, OperatorCategory operatorCategory, string path)
        {
            string batchRef = Guid.NewGuid().ToString("N") + DateTime.Now.Ticks.ToString() + path;
            CellSitesScheduleStaging staging = new CellSitesScheduleStaging
            {
                AddedByAdmin = adminUser != null ? true : false,
                AdminUser = adminUser,
                BatchRef = batchRef,
                FilePath = path,
                OperatorUser = loggedInUser,
                TaxProfile = taxProfile,
                PercentageProgress = 100,
                TotalNoOfRowsProcessed = count,
                FileName = System.IO.Path.GetFileName(path),
                OperatorCategory = operatorCategory
            };
            _cellSiteScheduleStagingService.Save(staging);
            staging.BatchRef = Util.ZeroPadUp(staging.Id.ToString(), 7, "Schedule");
            return staging;
        }
        

        /// <summary>
        /// Get the list of states and LGA transformed to <see cref="StatesAndLGAs"/>
        /// </summary>
        /// <returns>List<StatesAndLGAs></returns>
        public List<StatesAndLGAs> GetStatesAndLGAs()
        {
            try
            {
                //get the list of states and LGAs
                return _stateRepo.GetStatesAndLGAs();
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error getting states and LGAs"), exception.Message);
                throw;
            }
        }


    }
}