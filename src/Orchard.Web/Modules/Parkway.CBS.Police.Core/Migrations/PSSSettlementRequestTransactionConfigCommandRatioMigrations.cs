using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementRequestTransactionConfigCommandRatioMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementRequestTransactionConfigCommandRatio).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.ConfigTransaction) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.Command) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.State) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.LGA) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.CommandRatio), column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.Batch) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.Request) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.FallRatioFlag), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigCommandRatio.UpdatedAtUtc), column => column.NotNull())
                ).AlterTable(typeof(PSSSettlementRequestTransactionConfigCommandRatio).Name,
                    table => table.CreateIndex("PSRTCRATIO_GROUPBY_INDEX", new string[] { nameof(PSSSettlementRequestTransactionConfigCommandRatio.ConfigTransaction) + "_Id", nameof(PSSSettlementRequestTransactionConfigCommandRatio.Batch) + "_Id" }));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementRequestTransactionConfigCommandRatio).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSRTCRATIO_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Request)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandRatio.Command)}_Id], [{nameof(PSSSettlementRequestTransactionConfigCommandRatio.TransactionLog)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}