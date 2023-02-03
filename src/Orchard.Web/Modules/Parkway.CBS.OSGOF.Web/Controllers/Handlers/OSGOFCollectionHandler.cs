using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.DataFilters.CollectionReport;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.ViewModels;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.OSGOF.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Web.Controllers.Handlers
{
    public class OSGOFCollectionHandler : ModuleCollectionHandler, IOSGOFCollectionHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICoreCollectionService _coreCollectionService;
        //private readonly IEnumerable<IMDAFilter> _dataFilters;

        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly IEnumerable<IOSGOFBillingImpl> _billingImpls;
        private readonly ICollectionReportFilter _collectionReportFilter;
        private readonly ICoreUserService _coreUserService;
        private readonly ICoreReceiptService _coreReceiptService;
        public ILogger Logger { get; set; }
        private readonly ICorePaymentService _corePaymentService;
        private readonly Lazy<ICoreFormService> _coreFormService;


        public OSGOFCollectionHandler(IOrchardServices orchardServices, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreCollectionService coreCollectionService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, IEnumerable<IOSGOFBillingImpl> billingImpls, ICollectionReportFilter collectionreportFilter, ICoreReceiptService coreReceiptService, ICorePaymentService corePaymentService, Lazy<ICoreFormService> coreFormService) : base(orchardServices, taxCategoriesRepository, coreCollectionService, settingsRepository, handlerHelper, coreUserService, billingImpls, collectionreportFilter, coreReceiptService, corePaymentService, coreFormService)
        {
            _coreCollectionService = coreCollectionService;
            _taxCategoriesRepository = taxCategoriesRepository;
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
            _coreUserService = coreUserService;
            _billingImpls = billingImpls;
            _collectionReportFilter = collectionreportFilter;
            Logger = NullLogger.Instance;
            _coreReceiptService = coreReceiptService;
        }

    }
}