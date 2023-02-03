using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSAdminUsersMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSAdminUsers).Name,
                table => table
                    .Column<int>(nameof(PSSAdminUsers.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSSAdminUsers.Fullname), column => column.NotNull().WithLength(150))
                    .Column<int>(nameof(PSSAdminUsers.RoleType) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSAdminUsers.User) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSAdminUsers.PhoneNumber), column => column.NotNull().WithLength(13))
                    .Column<string>(nameof(PSSAdminUsers.Email), column => column.Nullable().WithLength(100))
                    .Column<int>(nameof(PSSAdminUsers.CommandCategory) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSAdminUsers.Command) + "_Id", column => column.Nullable())
                    .Column<int>(nameof(PSSAdminUsers.CreatedBy) + "_Id", column => column.Nullable())
                    .Column<DateTime>(nameof(PSSAdminUsers.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSAdminUsers.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSSAdminUsers).Name, table => table.AddColumn(nameof(PSSAdminUsers.LastUpdatedBy) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));

            SchemaBuilder.AlterTable(typeof(PSSAdminUsers).Name, table => table.AddColumn(nameof(PSSAdminUsers.IsActive), System.Data.DbType.Boolean, column => column.WithDefault(true)));

            string tableName = SchemaBuilder.TableDbName(typeof(PSSAdminUsers).Name);
            string queryString = string.Format("UPDATE {0} SET [{1}] = {2}", tableName, nameof(PSSAdminUsers.IsActive), 1);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format($"ALTER TABLE {tableName} ALTER COLUMN {nameof(PSSAdminUsers.IsActive)} bit NOT NULL");
            SchemaBuilder.ExecuteSql(queryString);

            return 2;

        }
    }
}