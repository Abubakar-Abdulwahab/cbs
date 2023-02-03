using Orchard.Data.Migration;
using Parkway.CBS.OSGOF.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Migrations
{
    public class CellSitesScheduleStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CellSitesScheduleStaging).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<bool>("AddedByAdmin", column => column.NotNull().WithDefault(false))
                            .Column<int>("AdminUser_Id", column => column.Nullable())
                            .Column<Int64>("OperatorUser_Id", column => column.Nullable())
                            .Column<Int64>("TaxProfile_Id", column => column.NotNull())
                            .Column<int>("OperatorCategory_Id", column => column.NotNull())
                            .Column<string>("FilePath", column => column.NotNull().WithLength(1000))
                            .Column<string>("FileName", column => column.NotNull().WithLength(260))
                            .Column<string>("BatchRef", column => column.NotNull().Unique().WithLength(250))
                            .Column<bool>("ErrorOccurred", column => column.NotNull().WithDefault(false))
                            .Column<string>("ErrorMessage", column => column.Nullable().Unlimited())
                            .Column<decimal>("Amount", column => column.Nullable())
                            .Column<bool>("TiedToTransaction", column => column.NotNull().WithDefault(false))
                            .Column<bool>("Treated", column => column.NotNull().WithDefault(false))
                            .Column<decimal>("PercentageProgress", column => column.NotNull().WithLength(500))
                            .Column<Int32>("TotalNoOfRowsProcessed", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }

    }
}