using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class SettlementRuleDetailsStagingMigrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(SettlementRuleDetailsStaging).Name,
               table => table
                           .Column<Int64>(nameof(SettlementRuleDetailsStaging.Id), column => column.PrimaryKey().Identity())
                           .Column<Int64>(nameof(SettlementRuleDetailsStaging.SettlementRuleStaging) + "_Id", column => column.NotNull())
                           .Column<int>(nameof(SettlementRuleDetailsStaging.MDA) + "_Id", column => column.NotNull())
                           .Column<int>(nameof(SettlementRuleDetailsStaging.RevenueHead) + "_Id", column => column.NotNull())
                           .Column<int>(nameof(SettlementRuleDetailsStaging.PaymentProvider) + "_Id", column => column.NotNull())
                           .Column<int>(nameof(SettlementRuleDetailsStaging.PaymentChannel_Id), column => column.NotNull())
                           .Column<bool>(nameof(SettlementRuleDetailsStaging.HasErrors), column => column.NotNull().WithDefault(false))
                           .Column<string>(nameof(SettlementRuleDetailsStaging.ErrorMessage), column => column.Nullable().WithLength(1000))
                           .Column<bool>(nameof(SettlementRuleDetailsStaging.IsOverride), column => column.NotNull().WithDefault(false))
                           .Column<Int64>(nameof(SettlementRuleDetailsStaging.OverrideSettlementRuleDetails) + "_Id", column => column.Nullable())
                           .Column<DateTime>(nameof(SettlementRuleDetailsStaging.CreatedAtUtc), column => column.NotNull())
                           .Column<DateTime>(nameof(SettlementRuleDetailsStaging.UpdatedAtUtc), column => column.NotNull())
               );
            return 1;
        }

    }
}