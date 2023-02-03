using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts
{
    public interface IRDCCreateInvoiceHandler : IDependency
    {
        APIResponse GenerateInvoice(RDCBillerCreateInvoiceModel model, object p);
    }
}
