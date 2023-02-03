using Orchard.Data.Migration;
using Parkway.CBS.OSGOF.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Migrations
{
    public class CellSitesStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CellSitesStaging).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("SNOnFile", column => column.NotNull())
                            .Column<string>("SNOnFileFileValue", column => column.Nullable().WithLength(100))
                            .Column<string>("OperatorSiteId", column => column.Nullable().WithLength(100))
                            .Column<int>("YearOfDeployment", column => column.Nullable())
                            .Column<string>("YearOfDeploymentFileValue", column => column.Nullable().WithLength(100))
                            .Column<decimal>("HeightOfTower", column => column.Nullable())
                            .Column<string>("HeightOfTowerFileValue", column => column.Nullable().WithLength(100))
                            .Column<string>("MastType", column => column.Nullable())
                            .Column<string>("Long", column => column.Nullable())
                            .Column<string>("Lat", column => column.Nullable())
                            .Column<string>("SiteAddress", column => column.Nullable().Unlimited())
                            .Column<string>("Region", column => column.Nullable())
                            .Column<int>("State_Id", column => column.Nullable())
                            .Column<string>("StateFileValue", column => column.Nullable())
                            .Column<int>("LGA_Id", column => column.Nullable())
                            .Column<string>("LGAFileValue", column => column.Nullable())
                            .Column<Int64>("Schedule_Id", column => column.NotNull())
                            .Column<bool>("HasErrors", column => column.WithDefault(false))
                            .Column<string>("ErrorMessages", column => column.Unlimited())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}