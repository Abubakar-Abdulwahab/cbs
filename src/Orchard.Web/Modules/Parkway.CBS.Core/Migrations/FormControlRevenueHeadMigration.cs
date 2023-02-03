using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class FormControlRevenueHeadMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(FormControlRevenueHead).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("MetaData", column => column.NotNull())                         
                            .Column<int>("RevenueHead_Id", column => column.NotNull())                  
                            .Column<int>("TaxEntityCategory_Id", column => column.NotNull())
                            .Column<DateTime>("DueDate", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(FormControlRevenueHead).Name, table => table.CreateIndex("RevenueHead_Id", new string[] { "RevenueHead_Id" }));
            SchemaBuilder.AlterTable(typeof(FormControlRevenueHead).Name, table => table.CreateIndex("TaxEntityCategory_Id", new string[] { "TaxEntityCategory_Id" }));
            SchemaBuilder.AlterTable(typeof(FormControlRevenueHead).Name, table => table.CreateIndex("GetForRH_TC", new string[] { "RevenueHead_Id", "TaxEntityCategory_Id" }));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(FormControlRevenueHead).Name, table => table.AddColumn("Form_Id", System.Data.DbType.Int32));
            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(FormControlRevenueHead).Name, table => table.AddColumn("IsComplusory", System.Data.DbType.Boolean));
            SchemaBuilder.AlterTable(typeof(FormControlRevenueHead).Name, table => table.AddColumn("Position", System.Data.DbType.Int32, col => col.WithDefault(0)));
            return 4;
        }

    }
}