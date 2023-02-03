using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSServiceStateManager<PSServiceState> : IDependency, IBaseManager<PSServiceState>
    { }
}
