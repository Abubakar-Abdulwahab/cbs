using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IServiceOptionHandler : IDependency
    {

        PSServiceOptionsPageVM GetOptionsVM(int serviceId);

        PSServiceOptionsVM GetSelectedOption(int serviceId, int? selectedOption);

        RouteNameAndStage GetNextOptionDirection(PSServiceOptionsVM option);

    }
}
