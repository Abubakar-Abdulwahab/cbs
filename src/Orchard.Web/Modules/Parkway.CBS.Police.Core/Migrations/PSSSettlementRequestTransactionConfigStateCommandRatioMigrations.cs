using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementRequestTransactionConfigStateCommandRatioMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementRequestTransactionConfigStateCommandRatio).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.TransactionLog) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.ConfigTransaction) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.StateCommand) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.State) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.LGA) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.StateCommandRatio), column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Batch) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Request) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.FallRatioFlag), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.UpdatedAtUtc), column => column.NotNull())
                ).AlterTable(typeof(PSSSettlementRequestTransactionConfigStateCommandRatio).Name,
                    table => table.CreateIndex("PSRTCRATIO_GROUPBY_INDEX", new string[] { nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.ConfigTransaction) + "_Id", nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Batch) + "_Id" }));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementRequestTransactionConfigStateCommandRatio).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSRTSTATECMDRATIO_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.Request)}_Id], [{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.StateCommand)}_Id], [{nameof(PSSSettlementRequestTransactionConfigStateCommandRatio.TransactionLog)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}