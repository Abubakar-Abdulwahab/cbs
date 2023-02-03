using Orchard;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Client.PSSServiceType.ServiceOptions.Contracts
{
    public interface IServiceOptionPresentation : IDependency
    {

        /// <summary>
        /// Get the option type
        /// </summary>
        string GetOptionType { get; }


        /// <summary>
        /// Get the route name and stage for this option
        /// </summary>
        RouteNameAndStage GetRouteName { get; }

    }
}
