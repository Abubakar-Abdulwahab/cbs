using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface IActivityPermissionSeeds : IDependency
    {
        /// <summary>
        /// Seeds two records to the ActivityPermission table
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns>void</returns>
        void SeedCBSPermissions(int currentUser);
    }
}
