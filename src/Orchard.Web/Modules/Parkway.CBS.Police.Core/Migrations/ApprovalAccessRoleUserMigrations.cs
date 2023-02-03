using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ApprovalAccessRoleUserMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ApprovalAccessRoleUser).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("ApprovalAccessRole_Id", column => column.NotNull())
                    .Column<int>("User_Id", column => column.NotNull())
                    .Column<int>("AddedBy_Id", column => column.NotNull())
                    .Column<bool>("IsActive", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(ApprovalAccessRoleUser).Name, table => table.DropColumn("ApprovalAccessRole_Id"));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(ApprovalAccessRoleUser).Name, table => table.AddColumn(nameof(ApprovalAccessRoleUser.AccessType), System.Data.DbType.Int32, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(ApprovalAccessRoleUser).Name);
            string queryString = string.Format("UPDATE {0} SET [AccessType] = {1}", tableName, (int)AdminUserType.Approver);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN AccessType int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }
    }
}