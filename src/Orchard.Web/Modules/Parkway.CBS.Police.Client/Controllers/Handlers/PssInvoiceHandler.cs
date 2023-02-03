using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;


namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class PssInvoiceHandler : IPssInvoiceHandler
    {
        private readonly ICoreCollectionService _coreCollectionService;

        public PssInvoiceHandler(ICoreCollectionService coreCollectionService)
        {
            _coreCollectionService = coreCollectionService;
            
        }

        /// <summary>
        /// get invoice URL
        /// </summary>
        /// <param name="bIN"></param>
        /// <returns>string</returns>
        /// <exception cref="NoInvoicesMatchingTheParametersFoundException"></exception>
        public string GetInvoiceURL(string bin)
        {
            return _coreCollectionService.GetInvoiceURL(bin);
        }


        public InvoiceGeneratedResponseExtn SearchForInvoiceForPaymentView(string bIN)
        { return _coreCollectionService.GetInvoiceGeneratedResponseObjectForPaymentView(bIN); }
    }
}