using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class StatsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(Stats).Name,
                table => table
                            .Column<Int64>("Id", column => column.Identity().PrimaryKey())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<Int64>("NumberOfInvoicesSent", column => column.NotNull().WithDefault(0))
                            .Column<decimal>("AmountExpected", column => column.NotNull().WithDefault(0))
                            .Column<decimal>("AmountPaid", column => column.NotNull().WithDefault(0))
                            .Column<Int64>("NumberOfInvoicesPaid", column => column.NotNull().WithDefault(0))
                            .Column<DateTime>("DueDate", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(Stats).Name, table => table.AddColumn("TaxEntityCategory_Id", System.Data.DbType.Int32));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(Stats).Name, table => table.AddColumn("StatsQueryConcat", System.Data.DbType.String));
            return 3;
        }
    }
}