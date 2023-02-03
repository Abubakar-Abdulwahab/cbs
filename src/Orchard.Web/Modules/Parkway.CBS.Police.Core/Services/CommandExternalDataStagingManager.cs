using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Police.Core.Services
{
    public class CommandExternalDataStagingManager : BaseManager<CommandExternalDataStaging>, ICommandExternalDataStagingManager<CommandExternalDataStaging>
    {
        private readonly IRepository<CommandExternalDataStaging> _repository;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public CommandExternalDataStagingManager(IRepository<CommandExternalDataStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Update the state_Id and LGA_Id columns in command staging table, 
        /// replace the HR statecode and lgacode to match POSSAP state and lga Id
        /// </summary>
        public void UpdateStateAndLGA()
        {
            try
            {
                var queryStateText = "UPDATE ceds SET ceds.StateCode=stateExternal.State_Id, ceds.UpdatedAtUtc=GETDATE() FROM Parkway_CBS_Police_Core_CommandExternalDataStaging ceds INNER JOIN Parkway_CBS_POSSAP_Scheduler_PSSStateModelExternalDataState stateExternal ON ceds.StateCode = stateExternal.ExternalDataStateCode";
                var queryState = _transactionManager.GetSession().CreateSQLQuery(queryStateText);
                queryState.ExecuteUpdate();

                var queryLGAText = "update ceds SET ceds.LGACode=lgaExternal.LGA_Id, ceds.UpdatedAtUtc=GETDATE() FROM Parkway_CBS_Police_Core_CommandExternalDataStaging ceds INNER JOIN Parkway_CBS_POSSAP_Scheduler_PSSLGAModelExternalDataLGA lgaExternal ON ceds.LGACode = lgaExternal.ExternalDataLGACode";
                var queryLGA = _transactionManager.GetSession().CreateSQLQuery(queryLGAText);
                queryLGA.ExecuteUpdate();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get external data caller log id
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="description"></param>
        /// <returns>int</returns>
        public int GetExternalDataCallerLog(string URL, string description)
        {
            try
            {
                string tableName = "Parkway_CBS_POSSAP_Scheduler_CallLogForExternalSystem";
                var queryStateText = $"INSERT INTO {tableName} (URL, CallDescription, CallStatus, CallIsSuccessful, CreatedAtUtc, UpdatedAtUtc) OUTPUT INSERTED.ID VALUES('{URL}', '{description}', 0, 1, GETDATE(), GETDATE())";
                var queryState = _transactionManager.GetSession().CreateSQLQuery(queryStateText);
                queryState.ExecuteUpdate();

                var getIdentity = $"SELECT IDENT_CURRENT('{tableName}')";
                var query = _transactionManager.GetSession().CreateSQLQuery(getIdentity);
                return Convert.ToInt32(query.UniqueResult());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Create the records that exist in CommandExternalDataStaging but doesn't exist in command
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        public void CreateNewRecordInCommandTable(long callLogForExternalSystemId)
        {
            try
            {
                var queryText = $"INSERT INTO Parkway_CBS_Police_Core_Command (Name, Code, CommandCategory_Id, State_Id, LGA_Id, AddedBy_Id, LastUpdatedBy_Id, Address, CommandType_Id, ParentCode, CreatedAtUtc, UpdatedAtUtc) SELECT ceds.Name, ceds.Code, ceds.CommandCategoryId, ceds.StateCode, ceds.LGACode, ceds.AddedBy, ceds.LastUpdatedBy, ceds.Address,  ceds.CommandTypeId, ceds.ParentCode, ceds.CreatedAtUtc, ceds.UpdatedAtUtc FROM Parkway_CBS_Police_Core_Command as cmd RIGHT JOIN Parkway_CBS_Police_Core_CommandExternalDataStaging as ceds ON cmd.Code = ceds.Code WHERE ceds.CallLogForExternalSystemId = :callLogForExternalSystemId AND ceds.HasError = :boolVal AND cmd.Code IS NULL;";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("callLogForExternalSystemId", callLogForExternalSystemId);
                query.SetParameter("boolVal", false);
                query.ExecuteUpdate();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Update records in command table that matches the code in CommandExternalDataStaging
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        public void UpdateRecordInCommandTable(long callLogForExternalSystemId)
        {
            try
            {
                var queryText = $"UPDATE cmd SET cmd.State_Id = ceds.StateCode, cmd.LGA_Id = ceds.LGACode, cmd.Address = ceds.Address FROM Parkway_CBS_Police_Core_Command cmd INNER JOIN Parkway_CBS_Police_Core_CommandExternalDataStaging ceds ON cmd.Code = ceds.Code WHERE ceds.CallLogForExternalSystemId = :callLogForExternalSystemId AND ceds.HasError = :boolVal;";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("callLogForExternalSystemId", callLogForExternalSystemId);
                query.SetParameter("boolVal", false);
                query.ExecuteUpdate();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///Update zonal code in command table that matches the zonalcode in CommandExternalDataStaging
        /// </summary>
        /// <param name="callLogForExternalSystemId"></param>
        public void UpdateZonalCodeInCommandTable(long callLogForExternalSystemId)
        {
            try
            {
                var queryText = $"UPDATE Parkway_CBS_Police_Core_Command SET ZonalCommand_Id = CMD.ID FROM (SELECT DISTINCT(CMD.Id), CEDS.Code from Parkway_CBS_Police_Core_Command CMD INNER JOIN Parkway_CBS_Police_Core_CommandExternalDataStaging CEDS ON cmd.Code = CEDS.ZonalCode where CEDS.CallLogForExternalSystemId= :callLogForExternalSystemId AND CEDS.HasError = :boolVal) AS CMD Where CMD.Code = Parkway_CBS_Police_Core_Command.Code;";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("callLogForExternalSystemId", callLogForExternalSystemId);
                query.SetParameter("boolVal", false);
                query.ExecuteUpdate();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}