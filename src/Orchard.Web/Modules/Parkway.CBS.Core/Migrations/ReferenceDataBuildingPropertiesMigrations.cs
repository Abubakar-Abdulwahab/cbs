using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class ReferenceDataBuildingPropertiesMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ReferenceDataBuildingProperties).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("ReferenceDataAsset_Id", column => column.NotNull())
                            .Column<int>("Purpose", column => column.NotNull())
                            .Column<int>("Structure", column => column.NotNull())
                            .Column<string>("Address", column => column.NotNull().WithLength(1000))
                            .Column<decimal>("RentAmount", column => column.Nullable())
                            .Column<int>("LGA_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}