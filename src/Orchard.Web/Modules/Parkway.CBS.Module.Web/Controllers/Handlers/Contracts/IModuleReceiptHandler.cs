using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.Contracts
{
    public interface IModuleReceiptHandler : IDependency, ICommonBaseHandler
    {

        InvoiceGeneratedResponseExtn SearchForInvoiceForPaymentView(string invoiceNumber);
    }
}
