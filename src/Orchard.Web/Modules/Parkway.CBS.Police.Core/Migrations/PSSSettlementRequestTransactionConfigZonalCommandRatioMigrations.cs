using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementRequestTransactionConfigZonalCommandRatioMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementRequestTransactionConfigZonalCommandRatio).Name,
               table => table
                   .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Id), column => column.PrimaryKey().Identity())
                   .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.TransactionLog) + "_Id", column => column.NotNull())
                   .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ConfigTransaction) + "_Id", column => column.NotNull())
                   .Column<int>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ZonalCommand) + "_Id", column => column.NotNull())
                   .Column<int>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.State) + "_Id", column => column.NotNull())
                   .Column<int>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.LGA) + "_Id", column => column.NotNull())
                   .Column<int>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ZonalCommandRatio), column => column.NotNull())
                   .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Batch) + "_Id", column => column.NotNull())
                   .Column<Int64>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Request) + "_Id", column => column.NotNull())
                   .Column<bool>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.FallRatioFlag), column => column.NotNull().WithDefault(false))
                   .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.CreatedAtUtc), column => column.NotNull())
                   .Column<DateTime>(nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.UpdatedAtUtc), column => column.NotNull())
               ).AlterTable(typeof(PSSSettlementRequestTransactionConfigZonalCommandRatio).Name,
                   table => table.CreateIndex("PSRTZCMDRATIO_GROUPBY_INDEX", new string[] { nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ConfigTransaction) + "_Id", nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Batch) + "_Id" }));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementRequestTransactionConfigZonalCommandRatio).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint PSRTZONALCMDRATIO_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.Request)}_Id], [{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.ZonalCommand)}_Id], [{nameof(PSSSettlementRequestTransactionConfigZonalCommandRatio.TransactionLog)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}