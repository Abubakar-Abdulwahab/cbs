using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ApprovalAccessListMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ApprovalAccessList).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("CommandCategory_Id", column => column.NotNull())
                    .Column<int>("State_Id", column => column.Nullable())
                    .Column<int>("Command_Id", column => column.Nullable())
                    .Column<int>("ApprovalAccessRoleUser_Id", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(ApprovalAccessList).Name, table => table.AddColumn("LGA_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(ApprovalAccessList).Name, table => table.AddColumn("Service_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(ApprovalAccessList).Name, table => table.AddColumn("IsDeleted", System.Data.DbType.Boolean, column => column.NotNull().WithDefault(false)));
            return 3;
        }

    }
}