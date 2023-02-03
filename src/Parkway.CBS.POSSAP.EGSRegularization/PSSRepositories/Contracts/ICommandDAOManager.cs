using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface ICommandDAOManager : IRepository<Command>
    {
        CommandVM GetCommandWithCode(string code);
    }
}
