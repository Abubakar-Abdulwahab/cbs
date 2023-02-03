using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Seeds.Contracts
{
    public interface IBankSeeds : IDependency
    {
        /// <summary>
        /// Populates banks in <see cref="Bank"/>
        /// </summary>
        /// <param name="banks"></param>
        void PopBanks(List<BankVM> banks);
    }
}
