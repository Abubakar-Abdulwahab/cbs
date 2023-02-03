using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class SettlementRuleDetailsMigrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(SettlementRuleDetails).Name,
                table => table
                            .Column<Int64>(nameof(SettlementRuleDetails.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(SettlementRuleDetails.SettlementRule) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(SettlementRuleDetails.MDA) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(SettlementRuleDetails.RevenueHead) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(SettlementRuleDetails.PaymentProvider) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(SettlementRuleDetails.PaymentChannel_Id), column => column.NotNull())
                            .Column<bool>(nameof(SettlementRuleDetails.IsDeleted), column => column.NotNull().WithDefault(false))
                            .Column<DateTime>(nameof(SettlementRuleDetails.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(SettlementRuleDetails.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }

    }
}