using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSSettlementFeePartyStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSSettlementFeePartyStaging).Name,
                table => table
                    .Column<Int64>(nameof(PSSSettlementFeePartyStaging.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PSSSettlementFeePartyStaging.Settlement) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementFeePartyStaging.FeeParty) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSSettlementFeePartyStaging.SN), column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlementFeePartyStaging.HasAdditionalSplits), column => column.NotNull().WithDefault(false))
                    .Column<bool>(nameof(PSSSettlementFeePartyStaging.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<string>(nameof(PSSSettlementFeePartyStaging.Reference), column => column.NotNull())
                    .Column<string>(nameof(PSSSettlementFeePartyStaging.AdditionalSplitValue), column => column.Nullable())
                    .Column<int>(nameof(PSSSettlementFeePartyStaging.DeductionTypeId), column => column.NotNull().WithDefault(0))
                    .Column<decimal>(nameof(PSSSettlementFeePartyStaging.DeductionValue), column => column.NotNull().WithDefault(0))
                    .Column<int>(nameof(PSSSettlementFeePartyStaging.Position), column => column.NotNull())
                    .Column<bool>(nameof(PSSSettlementFeePartyStaging.IsActive), column => column.NotNull().WithDefault(true))
                    .Column<bool>(nameof(PSSSettlementFeePartyStaging.MaxPercentage), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(PSSSettlementFeePartyStaging.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSSettlementFeePartyStaging.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSSettlementFeePartyStaging).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint PSSSettlementFeePartyStaging_Unique_Reference_SettlementId_SN_Constraint UNIQUE([{1}], [{2}], [{3}]); ", tableName, nameof(PSSSettlementFeePartyStaging.Settlement) + "_Id", nameof(PSSSettlementFeePartyStaging.SN), nameof(PSSSettlementFeePartyStaging.Reference)));

            return 1;
        }

    }
}