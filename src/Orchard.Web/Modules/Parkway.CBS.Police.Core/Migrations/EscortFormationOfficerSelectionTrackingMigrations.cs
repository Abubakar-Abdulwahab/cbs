using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortFormationOfficerSelectionTrackingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortFormationOfficerSelectionTracking).Name,
                table => table
                    .Column<Int64>(nameof(EscortFormationOfficerSelectionTracking.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(EscortFormationOfficerSelectionTracking.FormationAllocation) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(EscortFormationOfficerSelectionTracking.Group) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(EscortFormationOfficerSelectionTracking.PoliceOfficerLog) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(EscortFormationOfficerSelectionTracking.Reference), column => column.NotNull())
                    .Column<bool>(nameof(EscortFormationOfficerSelectionTracking.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(EscortFormationOfficerSelectionTracking.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortFormationOfficerSelectionTracking.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(EscortFormationOfficerSelectionTracking).Name, table => table.AddColumn(nameof(EscortFormationOfficerSelectionTracking.EscortRankRate), System.Data.DbType.Decimal, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(EscortFormationOfficerSelectionTracking).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = 0", tableName, nameof(EscortFormationOfficerSelectionTracking.EscortRankRate));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} decimal(19, 5) NOT NULL", tableName, nameof(EscortFormationOfficerSelectionTracking.EscortRankRate));
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }
    }
}