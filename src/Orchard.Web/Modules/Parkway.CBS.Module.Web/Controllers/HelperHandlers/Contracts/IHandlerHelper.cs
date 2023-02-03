using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.DTO;

namespace Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts
{

    public interface IHandlerHelper : IDependency
    {
        TenantCBSSettings GetTenant();

        List<StatesAndLGAs> GetAllStatesAndLgas();

        List<StateModel> GetAllStates();

        List<LGA> GetStateLgas(int StateId);

        bool CheckLgaExists(int lgaId);

    }
}