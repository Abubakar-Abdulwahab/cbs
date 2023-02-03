using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.Web.Controllers.Handlers
{
    public class ModuleReceiptHandler : CommonBaseHandler, IModuleReceiptHandler
    {
        private readonly ICoreReceiptService _coreReceiptService;
        private readonly ICoreInvoiceService _coreInvoiceService;

        public ModuleReceiptHandler(ICoreInvoiceService coreInvoiceService, IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, IFormControlsManager<FormControl> formcontrolsRepository, ICoreTaxPayerService coreTaxPayerService) : base(orchardServices, settingsRepository, handlerHelper)
        {
            _coreInvoiceService = coreInvoiceService;
        }


        public InvoiceGeneratedResponseExtn SearchForInvoiceForPaymentView(string invoiceNumber)
        {
            return _coreInvoiceService.GetInvoiceReceiptsVM(invoiceNumber);
        }

    }
}