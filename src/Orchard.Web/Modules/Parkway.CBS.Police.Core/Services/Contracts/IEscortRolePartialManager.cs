using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IEscortRolePartialManager<EscortRolePartial> : IDependency, IBaseManager<EscortRolePartial>
    {
        /// <summary>
        /// Get the VM for partials
        /// </summary>
        /// <returns>List{EscortPartialVM}</returns>
        IEnumerable<EscortPartialVM> GetPartials(int adminId);

        /// <summary>
        /// Get partials for role with specified id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        IEnumerable<EscortPartialVM> GetPartialsForRole(int roleId);

    }
}
