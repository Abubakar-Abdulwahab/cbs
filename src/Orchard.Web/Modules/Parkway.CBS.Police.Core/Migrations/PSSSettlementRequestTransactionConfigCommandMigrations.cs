using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementRequestTransactionConfigCommandMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementRequestTransactionConfigCommand).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommand.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommand.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommand.Command) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommand.Batch) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommand.Request) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommand.State) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommand.LGA) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommand.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommand.UpdatedAtUtc), column => column.NotNull())
                ).AlterTable(typeof(PSSSettlementRequestTransactionConfigCommand).Name,
                    table => table.CreateIndex("PSRTCC_GROUPBY_INDEX", new string[] { nameof(PSSSettlementRequestTransactionConfigCommand.Request) + "_Id", nameof(PSSSettlementRequestTransactionConfigCommand.TransactionLog) + "_Id", nameof(PSSSettlementRequestTransactionConfigCommand.Command) + "_Id", nameof(PSSSettlementRequestTransactionConfigCommand.Batch) + "_Id", nameof(PSSSettlementRequestTransactionConfigCommand.ConfigTransaction) + "_Id", nameof(PSSSettlementRequestTransactionConfigCommand.State) + "_Id", nameof(PSSSettlementRequestTransactionConfigCommand.LGA) + "_Id" }));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementRequestTransactionConfigCommand).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSRTCC_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementRequestTransactionConfigCommand.Request)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommand.Command)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommand.TransactionLog)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommand.RequestCommand)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}