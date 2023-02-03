using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIFormHandler : IDependency
    {
        APIResponse CreateFormControls(IntegrationController callback, CreateFormControls model, dynamic headerParams);
    }
}
