using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementAdapterCommandMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementAdapterCommand).Name,
                table => table
                    .Column<int>(nameof(PSSSettlementAdapterCommand.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSSSettlementAdapterCommand.ServiceCommand)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementAdapterCommand.SettlementCommand) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementAdapterCommand.FeePartyAdapter) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlementAdapterCommand.IsActive), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSSSettlementAdapterCommand.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementAdapterCommand.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementAdapterCommand).Name);

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE [dbo].[{0}] ADD constraint PSSSettlementAdapterCommand_ServiceCommand_SettlementCommand_FeePartyAdapter_Unique_Constraint UNIQUE([{1}], [{2}], [{3}]);", tableName, nameof(PSSSettlementAdapterCommand.ServiceCommand) + "_Id", nameof(PSSSettlementAdapterCommand.SettlementCommand) + "_Id", nameof(PSSSettlementAdapterCommand.FeePartyAdapter) + "_Id"));

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSSettlementAdapterCommand).Name, table => table.AddColumn($"{nameof(PSSSettlementAdapterCommand.SettlementAccountType)}", System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementAdapterCommand).Name);

            string queryString = string.Format("UPDATE {0} SET [SettlementAccountType] = {1}", tableName, (int)Models.Enums.SettlementAccountType.CommandSettlement);

            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} int NOT NULL", tableName, $"{nameof(PSSSettlementAdapterCommand.SettlementAccountType)}");
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }

    }
}