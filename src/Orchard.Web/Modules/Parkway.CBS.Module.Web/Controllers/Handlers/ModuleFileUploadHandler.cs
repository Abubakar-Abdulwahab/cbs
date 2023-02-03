using Orchard;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Core.Exceptions;
using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.FileUpload.Implementations.Contracts;

namespace Parkway.CBS.Module.Web.Controllers.Handlers
{
    public class ModuleFileUploadHandler : CommonBaseHandler, IModuleFileUploadHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICoreCollectionService _coreCollectionService;

        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly IFormControlRevenueHeadManager<FormControlRevenueHead> _formRevenueHeadRepository;
        private readonly IFormControlsManager<FormControl> _formcontrolsRepository;
        private readonly IEnumerable<IBillingImpl> _billingImpls;

        private readonly ICoreUserService _coreUserService;
        private readonly IFileUploadConfiguration _fileUploadConfig;


        public ModuleFileUploadHandler(IOrchardServices orchardServices, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreCollectionService coreCollectionService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, IFormControlsManager<FormControl> formcontrolsRepository, IEnumerable<IBillingImpl> billingImpls) : base(orchardServices, settingsRepository, handlerHelper)
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
            _fileUploadConfig = new FileUploadConfiguration();
        }        
    }
}