using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class CBSRolePermissionMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CBSRolePermission).Name,
                table => table
                            .Column<int>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("Permission_Id", column => column.NotNull())
                            .Column<int>("Role_Id", column => column.NotNull())
                            .Column<Int64>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}