using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortFormationAllocationMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortFormationAllocation).Name,
                table => table
                    .Column<Int64>(nameof(EscortFormationAllocation.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(EscortFormationAllocation.Group)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocation.EscortSquadAllocation) +"_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocation.AllocatedByAdminUser) +"_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocation.State)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocation.LGA)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocation.Command)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocation.NumberOfOfficers), column => column.NotNull())
                    .Column<int>(nameof(EscortFormationAllocation.NumberAssignedByCommander), column => column.NotNull())
                    .Column<string>(nameof(EscortFormationAllocation.StatusDescription), column => column.NotNull())
                    .Column<bool>(nameof(EscortFormationAllocation.Fulfilled), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortFormationAllocation.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortFormationAllocation.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(EscortFormationAllocation).Name, table => table.AddColumn(nameof(EscortFormationAllocation.IsDeleted), System.Data.DbType.Boolean, column => column.Nullable().WithDefault(false)));

            string tableName = SchemaBuilder.TableDbName(typeof(EscortFormationAllocation).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = 0", tableName, nameof(EscortFormationAllocation.IsDeleted));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} bit NOT NULL", tableName, nameof(EscortFormationAllocation.IsDeleted));
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }
    }
}