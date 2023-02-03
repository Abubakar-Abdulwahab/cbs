using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class EthnicityMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(Ethnicity).Name,
                table => table
                            .Column<int>(nameof(Ethnicity.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(Ethnicity.Name), column => column.NotNull())
                            .Column<bool>(nameof(Ethnicity.IsActive), column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}