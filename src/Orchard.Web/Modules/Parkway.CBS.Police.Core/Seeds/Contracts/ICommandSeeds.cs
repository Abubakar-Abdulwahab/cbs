using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.Seeds.Contracts
{
    public interface ICommandSeeds : IDependency
    {
        /// <summary>
        /// Add list of police commands to the database
        /// </summary>
        /// <returns>CommandStatVM</returns>
        CommandStatVM AddCommands(List<CommandVM> commands);
    }
}
