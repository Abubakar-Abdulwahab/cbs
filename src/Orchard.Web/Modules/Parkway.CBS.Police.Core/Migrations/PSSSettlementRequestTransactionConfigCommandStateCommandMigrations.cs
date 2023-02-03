using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementRequestTransactionConfigCommandStateCommandMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementRequestTransactionConfigCommandStateCommand).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.ConfigTransaction) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.StateCommand) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.RequestCommand) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.RequestAndCommand) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.State) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.LGA) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Batch) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Request) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.UpdatedAtUtc), column => column.NotNull())
                ).AlterTable(typeof(PSSSettlementRequestTransactionConfigCommandStateCommand).Name,
                    table => table.CreateIndex("PSRTSTATECOMMAND_GROUPBY_INDEX", new string[] { nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.ConfigTransaction) + "_Id", nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Batch) + "_Id" }));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementRequestTransactionConfigCommandStateCommand).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSRTSTATECOMMAND_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.ConfigTransaction)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.Request)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.RequestCommand)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.RequestAndCommand)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandStateCommand.TransactionLog)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}