using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class TestRefDataTINMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TIN).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("TINNumber", column => column.Unique().NotNull())
                            .Column<string>("FirstName", column => column.NotNull())
                            .Column<string>("LastName", column => column.NotNull())
                            .Column<string>("DOB", column => column.NotNull())
                            .Column<string>("Nationality", column => column.NotNull())
                            .Column<string>("State", column => column.NotNull())
                            .Column<string>("Address", column => column.NotNull())
                            .Column<string>("Occupation", column => column.NotNull()) //LastUpdatedBy
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(TIN).Name,
                    table => table.CreateIndex("TINNumber", new string[] { "TINNumber" }));
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(TIN).Name, table => table.AddColumn("CompanyName", System.Data.DbType.String));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(TIN).Name, table => table.AddColumn("PhoneNumber", System.Data.DbType.String))
                .AlterTable(typeof(TIN).Name, table => table.AddColumn("RCNumber", System.Data.DbType.String))
                .AlterTable(typeof(TIN).Name, table => table.AddColumn("Email", System.Data.DbType.String))
                .AlterTable(typeof(TIN).Name, table => table.AddColumn("CustomerType", System.Data.DbType.Int32))
                .AlterTable(typeof(TIN).Name, table => table.AddColumn("TaxEntityCategory_Id", System.Data.DbType.Int32));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(TIN).Name, table => table.AddColumn("RevenueHeadId", System.Data.DbType.Int32));
            return 4;
        }
    }
}