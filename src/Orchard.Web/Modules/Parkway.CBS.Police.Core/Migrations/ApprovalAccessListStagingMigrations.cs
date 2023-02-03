using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ApprovalAccessListStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ApprovalAccessListStaging).Name,
                table => table
                    .Column<long>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("CommandCategory_Id", column => column.NotNull())
                    .Column<int>("State_Id", column => column.Nullable())
                    .Column<int>("Command_Id", column => column.Nullable())
                    .Column<int>("ApprovalAccessRoleUser_Id", column => column.NotNull())
                    .Column<int>("LGA_Id", column => column.NotNull())
                    .Column<int>("Service_Id", column => column.NotNull())
                    .Column<bool>("IsDeleted", column => column.NotNull())
                    .Column<string>("Reference", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

    }
}