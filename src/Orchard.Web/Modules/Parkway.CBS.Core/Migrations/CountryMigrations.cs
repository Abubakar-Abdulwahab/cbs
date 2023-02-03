using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
namespace Parkway.CBS.Core.Migrations
{
    public class CountryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(Country).Name,
                table => table
                            .Column<int>(nameof(Country.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(Country.Name), column => column.NotNull())
                            .Column<string>(nameof(Country.Code), column => column.NotNull())
                            .Column<bool>(nameof(Country.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}