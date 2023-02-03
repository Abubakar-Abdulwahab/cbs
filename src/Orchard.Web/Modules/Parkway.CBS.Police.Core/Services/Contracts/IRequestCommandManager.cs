using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IRequestCommandManager<RequestCommand> : IDependency, IBaseManager<RequestCommand>
    {
    }
}
