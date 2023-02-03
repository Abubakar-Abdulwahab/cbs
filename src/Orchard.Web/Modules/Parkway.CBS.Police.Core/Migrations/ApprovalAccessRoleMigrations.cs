using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ApprovalAccessRoleMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ApprovalAccessRole).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", column => column.NotNull().Unique())
                    .Column<int>("AddedBy_Id", column => column.NotNull())
                    .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}