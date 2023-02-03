using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class SettlementRuleStagingMigrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(SettlementRuleStaging).Name,
                table => table
                            .Column<Int64>(nameof(SettlementRuleStaging.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(SettlementRuleStaging.Name), column => column.NotNull().Unique())
                            .Column<string>(nameof(SettlementRuleStaging.SettlementEngineRuleIdentifier), column => column.NotNull().WithLength(100))
                            .Column<int>(nameof(SettlementRuleStaging.AddedBy) + "_Id", column => column.NotNull())
                            .Column<string>(nameof(SettlementRuleStaging.CronExpression), column => column.NotNull().WithLength(250))
                            .Column<DateTime>(nameof(SettlementRuleStaging.SettlementPeriodStartDate), column => column.NotNull())
                            .Column<DateTime>(nameof(SettlementRuleStaging.SettlementPeriodEndDate), column => column.NotNull())
                            .Column<string>(nameof(SettlementRuleStaging.JSONScheduleModel), column => column.NotNull().WithLength(1000))
                            .Column<bool>(nameof(SettlementRuleStaging.HasDoneValidation), column => column.NotNull().WithDefault(false))
                            .Column<bool>(nameof(SettlementRuleStaging.IsEdit), column => column.NotNull().WithDefault(false))
                            .Column<int>(nameof(SettlementRuleStaging.SettlementRuleToBeEdited)+"_Id", column => column.Nullable())
                            .Column<DateTime>(nameof(SettlementRuleStaging.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(SettlementRuleStaging.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }

    }
}