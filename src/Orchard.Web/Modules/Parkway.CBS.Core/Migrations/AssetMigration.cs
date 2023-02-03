using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class AssetMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(Asset).Name, table => table
               .Column<int>("Id", c => c.Identity().PrimaryKey())
               .Column<string>("TypeOfAsset")
               .Column<decimal>("MarketValue")
               .Column<DateTime>("OwnershipDate")
               .Column<string>("LocationOfAsset")
               .Column("TIN_Id", System.Data.DbType.Int32)
               .Column<DateTime>("CreatedAtUtc", c => c.Nullable())
               .Column<DateTime>("UpdatedAtUtc", c => c.Nullable())
              );
            return 1;
        }
    }
}