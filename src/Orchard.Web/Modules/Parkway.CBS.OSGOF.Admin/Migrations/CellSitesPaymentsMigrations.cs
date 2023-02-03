using Orchard.Data.Migration;
using Parkway.CBS.OSGOF.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Migrations
{
    public class CellSitesPaymentsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CellSitesPayment).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("CellSiteClientPaymentBatch_Id", column => column.Nullable())
                            .Column<Int64>("CellSite_Id", column => column.Nullable())
                            .Column<string>("CellSiteId", column => column.Nullable().WithLength(100))
                            .Column<string>("Reference", column => column.Nullable().Unlimited())
                            .Column<int>("Year", column => column.Nullable())
                            .Column<string>("YearStringValue", column => column.Nullable().WithLength(50))
                            .Column<bool>("HasErrors", column => column.WithDefault(false).NotNull())
                            .Column<string>("ErrorMessage", column => column.Nullable().Unlimited())
                            .Column<decimal>("Amount", column => column.NotNull())
                            .Column<DateTime>("AssessmentDate", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(CellSitesPayment).Name,
                    table => table.CreateIndex("CellSiteId", new string[] { "CellSiteId" }));
            return 1;
        }
    }
}