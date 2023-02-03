using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class SettlementRuleMigrations : DataMigrationImpl
    {

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(SettlementRule).Name,
                table => table
                            .Column<int>(nameof(SettlementRule.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(SettlementRule.Name), column => column.NotNull().Unique().WithLength(100))
                            .Column<string>(nameof(SettlementRule.SettlementEngineRuleIdentifier), column => column.NotNull().WithLength(100))
                            .Column<int>(nameof(SettlementRule.AddedBy) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(SettlementRule.ConfirmedBy) + "_Id", column => column.NotNull())
                            .Column<string>(nameof(SettlementRule.CronExpression), column => column.NotNull().WithLength(250))
                            .Column<DateTime>(nameof(SettlementRule.SettlementPeriodStartDate), column => column.NotNull())
                            .Column<DateTime>(nameof(SettlementRule.SettlementPeriodEndDate), column => column.NotNull())
                            .Column<DateTime>(nameof(SettlementRule.NextScheduleDate), column => column.NotNull())
                            .Column<int>(nameof(SettlementRule.NumberOfRuns), column => column.NotNull())
                            .Column<string>(nameof(SettlementRule.JSONScheduleModel), column => column.NotNull().WithLength(1000))
                            .Column<bool>(nameof(SettlementRule.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(SettlementRule.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(SettlementRule.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(SettlementRule).Name, table => table.AddColumn(nameof(SettlementRule.SettlementCycle), System.Data.DbType.Int32, column => column.WithDefault(1)));

            string tableName = SchemaBuilder.TableDbName(typeof(SettlementRule).Name);
            string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} int NOT NULL", tableName, nameof(SettlementRule.SettlementCycle));
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }

    }
}