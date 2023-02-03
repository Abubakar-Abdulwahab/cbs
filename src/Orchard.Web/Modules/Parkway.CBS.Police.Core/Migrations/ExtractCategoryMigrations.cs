using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ExtractCategoryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ExtractCategory).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", column => column.NotNull().Unique())
                    .Column<bool>("FreeForm", column => column.NotNull())
                    .Column<bool>("IsActive", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}