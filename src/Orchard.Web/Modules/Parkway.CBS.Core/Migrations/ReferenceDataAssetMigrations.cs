using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class ReferenceDataAssetMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ReferenceDataAsset).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("AssetType_Id", column => column.NotNull())
                            .Column<string>("Fullname", column => column.NotNull())
                            .Column<string>("PhoneNumber", column => column.NotNull())
                            .Column<string>("Email", column => column.NotNull())
                            .Column<string>("Address", column => column.NotNull().WithLength(1000))
                            .Column<string>("TIN", column => column.Nullable())
                            .Column<int>("LGA_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}