using Orchard.Logging;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.Contracts;
using System;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSHRExternalDataCommandSeed : IPSSHRExternalDataCommandSeed
    {
        private readonly IExternalDataCommands _externalDataCommands;
        public ILogger Logger { get; set; }

        public PSSHRExternalDataCommandSeed(IExternalDataCommands externalDataCommands)
        {
            Logger = NullLogger.Instance;
            _externalDataCommands = externalDataCommands;
        }

        /// <summary>
        /// Get HR list of commands
        /// </summary>
        /// <returns>string</returns>
        public string GetHRCommands()
        {
            try
            {
                return _externalDataCommands.GetCommands();
            }
            catch (Exception)
            {
                return "Error!!!";
            }
        }
    }
}