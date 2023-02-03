using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Web.Controllers.Handlers
{
    public class OSGOFCollectionAjaxHandler : ModuleCollectionAjaxHandler, IOSGOFCollectionAjaxHandler
    {

        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICoreCollectionService _coreCollectionService;

        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly IFormControlRevenueHeadManager<FormControlRevenueHead> _formRevenueHeadRepository;
        private readonly IFormControlsManager<FormControl> _formcontrolsRepository;
        private readonly IEnumerable<IBillingImpl> _billingImpls;
        private readonly ICorePaymentService _corePaymentService;
        private readonly ICoreUserService _coreUserService;
        private readonly ICoreTaxPayerService _coreTaxPayerService;
        private readonly ICellSiteManager<CellSites> _cellSiteManager;

        public OSGOFCollectionAjaxHandler(IOrchardServices orchardServices, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreCollectionService coreCollectionService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, IFormControlsManager<FormControl> formcontrolsRepository, IEnumerable<IBillingImpl> billingImpls, ICoreTaxPayerService coreTaxPayerService, ICellSiteManager<CellSites> cellSiteManager, ICorePaymentService corePaymentService) : base(orchardServices, taxCategoriesRepository, coreCollectionService, settingsRepository, handlerHelper, coreUserService, formRevenueHeadRepository, formcontrolsRepository, billingImpls, coreTaxPayerService, corePaymentService)
        {
            _coreCollectionService = coreCollectionService;
            _taxCategoriesRepository = taxCategoriesRepository;
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
            _coreUserService = coreUserService;
            _formRevenueHeadRepository = formRevenueHeadRepository;
            _formcontrolsRepository = formcontrolsRepository;
            _billingImpls = billingImpls;
            _coreTaxPayerService = coreTaxPayerService;
            _cellSiteManager = cellSiteManager;
        }


        public APIResponse GetCellSite(string cellSiteId)
        {
            int operatorCellSiteId = 0;
            bool parsedCellSiteId = Int32.TryParse(cellSiteId, out operatorCellSiteId);
            if (!parsedCellSiteId) { return new APIResponse { Error = true, ResponseObject = ErrorLang.nocellsiterecord404(cellSiteId).ToString() }; }

            CellSitesDetailsVM cellSite = _cellSiteManager.GetCellSite(operatorCellSiteId);
            if (cellSite == null) { return new APIResponse { Error = true, ResponseObject = Lang.noprofiles404.ToString() }; }
            return new APIResponse { ResponseObject = cellSite };
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <param name="LGAId"></param>
        /// <returns></returns>
        public APIResponse GetOperatorCellSites(string taxEntityId, string LGAId)
        {
            int operatorId = 0, operatorLGAId = 0;
            bool parsedoperatorId = Int32.TryParse(taxEntityId, out operatorId);
            if (!parsedoperatorId) { return new APIResponse { Error = true, ResponseObject = ErrorLang.categorynotfound().ToString() }; }

            bool parsedoperatorLGAId = Int32.TryParse(LGAId, out operatorLGAId);
            if (!parsedoperatorLGAId) { return new APIResponse { Error = true, ResponseObject = ErrorLang.nolgarecord404(LGAId).ToString() }; }

            List<CellSitesDropdownBindingVM> cellSites = _cellSiteManager.GetOperatorCellSites(operatorId, operatorLGAId);
            if (cellSites == null || cellSites.Count <= 0) { return new APIResponse { Error = true, ResponseObject = Lang.noprofiles404.ToString() }; }
            return new APIResponse { ResponseObject = cellSites };
        }
    }
}