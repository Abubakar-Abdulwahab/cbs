using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementRequestTransactionConfigCommandZonalCommandMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementRequestTransactionConfigCommandZonalCommand).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ConfigTransaction) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ZonalCommand) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.RequestCommand) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.RequestAndCommand) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.State) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.LGA) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Batch) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Request) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.UpdatedAtUtc), column => column.NotNull())
                ).AlterTable(typeof(PSSSettlementRequestTransactionConfigCommandZonalCommand).Name,
                    table => table.CreateIndex("PSRTZONALCOMMAND_GROUPBY_INDEX", new string[] { nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ConfigTransaction) + "_Id", nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Batch) + "_Id" }));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementRequestTransactionConfigCommandZonalCommand).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSRTZONALCOMMAND_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.ConfigTransaction)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.Request)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.RequestCommand)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.RequestAndCommand)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandZonalCommand.TransactionLog)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}