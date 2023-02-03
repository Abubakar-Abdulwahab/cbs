using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class CBSUserRoleMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CBSUserRole).Name,
                table => table
                            .Column<int>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("User_Id", column => column.NotNull())
                            .Column<int>("Role_Id", column => column.NotNull())
                            .Column<Int64>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}