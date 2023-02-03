using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxEntityCategoryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxEntityCategory).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Name", column => column.NotNull().Unique())
                            .Column<bool>("Status", column => column.NotNull().WithDefault(false))
                            .Column<int>("Identifier", column => column.NotNull().Unique())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(TaxEntityCategory).Name,
                    table => table.CreateIndex("Name", new string[] { "Name" }))
                    .AlterTable(typeof(TaxEntityCategory).Name,
                    table => table.CreateIndex("Identifier", new string[] { "Identifier" }));
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntityCategory).Name, table => table.AddColumn("RequiresLogin", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntityCategory).Name, table => table.AddColumn("StringIdentifier", System.Data.DbType.String));
            return 3;
        }


        public int UpdateFrom3()
        {
            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntityCategory).Name, table => table.AddColumn("JSONSettings", System.Data.DbType.String, column => column.Unlimited()));
            return 5;
        }
    }
}