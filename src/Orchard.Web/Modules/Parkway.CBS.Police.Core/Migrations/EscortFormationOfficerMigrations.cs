using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortFormationOfficerMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortFormationOfficer).Name,
                table => table
                    .Column<Int64>(nameof(EscortFormationOfficer.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(EscortFormationOfficer.FormationAllocation)+"_Id", column => column.NotNull())
                    .Column<Int64>(nameof(EscortFormationOfficer.Group)+ "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(EscortFormationOfficer.PoliceOfficerLog) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(EscortFormationOfficer.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(EscortFormationOfficer.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortFormationOfficer.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(EscortFormationOfficer).Name, table => table.AddColumn(nameof(EscortFormationOfficer.EscortRankRate), System.Data.DbType.Decimal, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(EscortFormationOfficer).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = 0", tableName, nameof(EscortFormationOfficer.EscortRankRate));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} decimal(19, 5) NOT NULL", tableName, nameof(EscortFormationOfficer.EscortRankRate));
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }
    }
}