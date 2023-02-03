using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class TenantCBSSettingsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TenantCBSSettings).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Identifier", column => column.Unique().WithLength(50).NotNull())
                            .Column<string>("StateName", column => column.Unique().WithLength(50).NotNull())
                            .Column<int>("StateId", column => column.NotNull().Unique())
                            .Column<string>("CountryId", column => column.NotNull().Unique())
                            .Column<int>("DefaultLGA_Id", column => column.NotNull().Unique())
                            .Column<string>("CountryName", column => column.NotNull().Unique())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<int>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }

    }
}