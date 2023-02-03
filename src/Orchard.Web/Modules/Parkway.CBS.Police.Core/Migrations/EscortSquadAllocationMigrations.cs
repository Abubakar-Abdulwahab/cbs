using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortSquadAllocationMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortSquadAllocation).Name,
                table => table
                    .Column<Int64>(nameof(EscortSquadAllocation.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(EscortSquadAllocation.Command) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortSquadAllocation.NumberOfOfficers), column => column.NotNull())
                    .Column<string>(nameof(EscortSquadAllocation.StatusDescription), column => column.NotNull())
                    .Column<bool>(nameof(EscortSquadAllocation.Fulfilled), column => column.NotNull())
                    .Column<Int64>(nameof(EscortSquadAllocation.AllocationGroup)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(EscortSquadAllocation.CommandType)+"_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(EscortSquadAllocation.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(EscortSquadAllocation.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(EscortSquadAllocation).Name, table => table.AddColumn(nameof(EscortSquadAllocation.IsDeleted), System.Data.DbType.Boolean, column => column.Nullable().WithDefault(false)));

            string tableName = SchemaBuilder.TableDbName(typeof(EscortSquadAllocation).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = 0", tableName, nameof(EscortSquadAllocation.IsDeleted));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} bit NOT NULL", tableName, nameof(EscortSquadAllocation.IsDeleted));
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }
    }
}