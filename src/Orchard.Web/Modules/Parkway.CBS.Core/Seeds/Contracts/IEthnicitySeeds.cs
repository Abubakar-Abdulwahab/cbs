using Orchard;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Seeds.Contracts
{
    public interface IEthnicitySeeds : IDependency
    {
        /// <summary>
        /// Adds a collection of ethnicities
        /// </summary>
        /// <param name="ethnicities"></param>
        void AddEthnicities(IEnumerable<dynamic> ethnicities);
    }
}
