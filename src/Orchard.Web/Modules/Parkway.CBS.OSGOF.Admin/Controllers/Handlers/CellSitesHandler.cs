using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Users.Models;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.OSGOF.Admin.CoreServices.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using Parkway.CBS.OSGOF.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using System.IO;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Entities.VMs;

namespace Parkway.CBS.OSGOF.Admin.Controllers.Handlers
{
    public class CellSitesHandler : BaseHandler, ICellSitesHandler
    {
        public Localizer T { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _entityCategoryRepository;
        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;
        private readonly ICoreUserService _coreUserService;
        private readonly ICoreCellSites _coreCellSites;
        private readonly IStateModelManager<StateModel> _stateModelManager;
        private readonly ICellSiteManager<CellSites> _cellSitesService;


        public ILogger Logger { get; set; }

        public CellSitesHandler(IOrchardServices orchardServices, ICoreUserService coreService, ICellSiteManager<CellSites> cellSitesService, ITaxEntityCategoryManager<TaxEntityCategory> entityCategoryRepository, ITaxEntityManager<TaxEntity> taxEntityManager, IStateModelManager<StateModel> stateRepo, ICoreCellSites coreCellSites, IStateModelManager<StateModel> stateModelManager) : base(orchardServices)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            _entityCategoryRepository = entityCategoryRepository;
            _coreUserService = coreService;
            Logger = NullLogger.Instance;
            _taxEntityManager = taxEntityManager;
            _coreCellSites = coreCellSites;
            _stateModelManager = stateModelManager;
            _cellSitesService = cellSitesService;
        }

        public void CheckForPermission(Permission permission)
        {
            IsAuthorized<CellSitesHandler>(permission);
        }

        /// <summary>
        /// Get the paged data of cell sites staging
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>CellSitesStagingReportVM</returns>
        public CellSitesStagingReportVM GetScheduleStagingData(string scheduleBatchRef, int take, int skip)
        {
            return _coreCellSites.GetCellSitesStaging(scheduleBatchRef, take, skip);
        }


        public CellSitesStagingReportVM CompleteCellSiteProcessing(string scheduleRef)
        {
            return _coreCellSites.MoveCellSitesFromStagingToMainTable(scheduleRef, new UserPartRecord { Id = _orchardServices.WorkContext.CurrentUser.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
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
                        //Operator = x.TaxProfile,
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

                return _coreCellSites.StoreCellSitesIntoStaging(new TaxEntity { Id = taxProfileDetails.Id, TaxEntityCategory = new TaxEntityCategory { Id = taxProfileDetails.CategoryId } }, adminUser, loggedInUser, path);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw;
            }
        }


    }
}