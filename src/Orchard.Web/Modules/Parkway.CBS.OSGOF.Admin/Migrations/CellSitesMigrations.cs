using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.OSGOF.Admin.Models;

namespace Parkway.CBS.OSGOF.Admin.Migrations
{
    public class CellSitesMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CellSites).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<bool>("AddedByAdmin", column => column.WithDefault(false))
                            .Column<int>("AdminUser_Id", column => column.Nullable())
                            .Column<Int64>("OperatorUser_Id", column => column.Nullable())
                            .Column<Int64>("TaxProfile_Id", column => column.NotNull())
                            .Column<int>("OperatorCategory_Id", column => column.NotNull())
                            .Column<string>("OperatorSiteId", column => column.NotNull().WithLength(100))
                            .Column<string>("OperatorSitePrefix", column => column.NotNull().WithLength(50))
                            .Column<int>("YearOfDeployment", column => column.NotNull())
                            .Column<decimal>("HeightOfTower", column => column.Nullable())
                            .Column<string>("MastType", column => column.NotNull())
                            .Column<string>("Long", column => column.NotNull())
                            .Column<string>("Lat", column => column.NotNull())
                            .Column<string>("SiteAddress", column => column.NotNull().WithLength(500))
                            .Column<string>("Region", column => column.Nullable())
                            .Column<int>("State_Id", column => column.NotNull())
                            .Column<int>("LGA_Id", column => column.NotNull())
                            .Column<bool>("Approved", column => column.NotNull().WithDefault(false))
                            .Column<int>("ApprovedBy_Id", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(CellSites).Name);
            string queryString = string.Format("ALTER TABLE {0} add [OSGOFId] as ((concat('OSGOF_',[OperatorSitePrefix],case when len(CONVERT([varchar](10),[Id]))<(3) then '_000' else '_' end,[Id]))) PERSISTED NOT NULL", tableName);
            return 1;
        }
    }
}