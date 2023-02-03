using Orchard;

namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.Contracts
{
    public interface IExternalDataCommands : IDependency
    {
        /// <summary>
        /// Get commands from the HR external data source and populate the command table
        /// </summary>
        /// <returns>string</returns>
        string GetCommands();
    }
}