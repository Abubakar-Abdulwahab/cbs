using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementFeePartyMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementFeeParty).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementFeeParty.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementFeeParty.Settlement) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementFeeParty.FeeParty) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlementFeeParty.HasAdditionalSplits), column => column.NotNull().WithDefault(false))
                    .Column<bool>(nameof(PSSSettlementFeeParty.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<string>(nameof(PSSSettlementFeeParty.AdditionalSplitValue), column => column.Nullable())
                    .Column<int>(nameof(PSSSettlementFeeParty.DeductionTypeId), column => column.NotNull().WithDefault(0))
                    .Column<decimal>(nameof(PSSSettlementFeeParty.DeductionValue), column => column.NotNull().WithDefault(0))
                    .Column<int>(nameof(PSSSettlementFeeParty.Position), column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlementFeeParty.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<bool>(nameof(PSSSettlementFeeParty.MaxPercentage), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSSSettlementFeeParty.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementFeeParty.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementFeeParty).Name);

            string queryString = $"ALTER TABLE[dbo].[{tableName}] ADD constraint SETTLEMENTFEEPARTY_UNIQUE_CONSTRAINT UNIQUE([{nameof(PSSSettlementFeeParty.FeeParty)}_Id], [{nameof(PSSSettlementFeeParty.Settlement)}_Id]);";

            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}