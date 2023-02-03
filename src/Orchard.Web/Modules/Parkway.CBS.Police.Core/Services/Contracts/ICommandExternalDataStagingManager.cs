using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ICommandExternalDataStagingManager<CommandExternalDataStaging> : IDependency, IBaseManager<CommandExternalDataStaging>
    {
        /// <summary>
        /// Update the state_Id and LGA_Id columns in command staging table, 
        /// replace the HR statecode and lgacode to match POSSAP state and lga Id
        /// </summary>
        void UpdateStateAndLGA();

        /// <summary>
        /// Get external data caller log id
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="description"></param>
        /// <returns>int</returns>
        int GetExternalDataCallerLog(string URL, string description);

        /// <summary>
        ///Update records in command table that matches the code in CommandExternalDataStaging
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        void UpdateRecordInCommandTable(long callLogForExternalSystemId);

        /// <summary>
        ///Create the records that exist in CommandExternalDataStaging but doesn't exist in command
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        void CreateNewRecordInCommandTable(long callLogForExternalSystemId);

        /// <summary>
        ///Update zonal code in command table that matches the zonalcode in CommandExternalDataStaging
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        void UpdateZonalCodeInCommandTable(long callLogForExternalSystemId);
    }
}
