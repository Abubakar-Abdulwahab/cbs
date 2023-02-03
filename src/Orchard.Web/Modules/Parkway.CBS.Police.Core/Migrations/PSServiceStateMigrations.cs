using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceStateMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceState).Name,
                table => table
                    .Column<int>(nameof(PSServiceState.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSServiceState.State)+ "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSServiceState.Service)+ "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSServiceState.IsActive), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSServiceState.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSServiceState.UpdatedAtUtc), column => column.NotNull())
                );

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE [dbo].[{0}] ADD constraint PSServiceState_Unique_Constraint UNIQUE([{1}], [{2}]);", SchemaBuilder.TableDbName(typeof(PSServiceState).Name), nameof(PSServiceState.State) + "_Id", nameof(PSServiceState.Service) + "_Id"));
            return 1;
        }


        public int UpdateFrom1()
        {
            try
            {
                var queryString = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSServiceState)} " +
                    $"({nameof(PSServiceState.State)}_Id, {nameof(PSServiceState.Service)}_Id, {nameof(PSServiceState.CreatedAtUtc)}, {nameof(PSServiceState.UpdatedAtUtc)}) SELECT state.{nameof(StateModel.Id)}, service.{nameof(PSService.Id)}, GETDATE(), GETDATE() FROM Parkway_CBS_Core_{nameof(StateModel)} state INNER JOIN Parkway_CBS_Police_Core_{nameof(PSService)} service ON state.{nameof(StateModel.IsActive)} = 1;";

                SchemaBuilder.ExecuteSql(queryString);

                queryString = $"INSERT INTO Parkway_CBS_Police_Core_{nameof(PSServiceStateCommand)} " +
                   $"({nameof(PSServiceStateCommand.ServiceState)}_Id, {nameof(PSServiceStateCommand.Command)}_Id, {nameof(PSServiceStateCommand.CreatedAtUtc)}, {nameof(PSServiceStateCommand.UpdatedAtUtc)}) SELECT stateServ.{nameof(PSServiceState.Id)}, command.{nameof(Command.Id)}, GETDATE(), GETDATE() FROM Parkway_CBS_Police_Core_{nameof(PSServiceState)} stateServ INNER JOIN Parkway_CBS_Police_Core_{nameof(Command)} command ON command.{nameof(Command.State)}_Id = stateServ.{nameof(PSServiceState.State)}_Id;";

                SchemaBuilder.ExecuteSql(queryString);

                return 2;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}