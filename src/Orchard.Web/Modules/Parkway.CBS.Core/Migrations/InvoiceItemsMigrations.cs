using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class InvoiceItemsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(InvoiceItems).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("InvoiceNumber", column => column.NotNull())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<Int64>("Invoice_Id", column => column.NotNull())
                            .Column<decimal>("UnitAmount", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<int>("TaxEntityCategory_Id", column => column.NotNull())
                            .Column<int>("Quantity", column => column.NotNull().WithDefault(0))
                            .Column<string>("InvoicingUniqueIdentifier", column => column.Nullable().WithLength(250))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(InvoiceItems).Name);

            string queryString = string.Format("ALTER TABLE {0} add [TotalAmount] as (([UnitAmount]*[Quantity])) ", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}