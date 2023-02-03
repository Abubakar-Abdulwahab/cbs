using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class APIRequestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(APIRequest).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<short>("CallType", column => column.NotNull())
                            .Column<string>("RequestIdentifier", column => column.NotNull().WithLength(100))
                            .Column<Int64>("ResourceIdentifier", column => column.NotNull())
                            .Column<int>("ExpertSystemSettings_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(APIRequest).Name,
                    table => table.CreateIndex("NCI_CheckRequest", new string[] { "RequestIdentifier", "CallType", "ExpertSystemSettings_Id" }))
                    .AlterTable(typeof(APIRequest).Name, 
                    table => table.AddUniqueConstraint("TC_UniqueCallTypeRequest", new string[] { "RequestIdentifier", "CallType", "ExpertSystemSettings_Id" }));
            return 1;
        }
    }
}