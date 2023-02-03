using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class MDASettingsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(MDASettings).Name,
                    table => table
                                .Column<int>("Id", column => column.Identity().PrimaryKey())
                                .Column<string>("CompanyAddress", column => column.Nullable())
                                .Column<string>("CompanyEmail", column => column.NotNull())
                                .Column<string>("CompanyMobile", column => column.Nullable())
                                .Column<string>("BusinessNature", column => column.Nullable())
                                .Column<string>("LogoPath", column => column.Nullable())
                                .Column<string>("SignaturePath", column => column.Nullable())
                                .Column<int>("CountryID", column => column.NotNull())
                                .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                                .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                    );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(MDASettings).Name, table => table.DropColumn("CountryID"));
            return 2;
        }
    }
}