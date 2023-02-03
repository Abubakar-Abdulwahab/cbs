using Orchard;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSHRExternalDataCommandSeed : IDependency
    {
        /// <summary>
        /// Get HR list of commands
        /// </summary>
        /// <returns>string</returns>
        string GetHRCommands();
    }
}
