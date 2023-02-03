using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class FormControlRevenueHeadValueMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(FormControlRevenueHeadValue).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("FormControlRevenueHead_Id", column => column.NotNull())
                            .Column<Int64>("Invoice_Id", column => column.NotNull())
                            .Column<Int64>("InvoiceItem_Id", column => column.NotNull())
                            .Column<string>("Value", column => column.NotNull())
                            .Column<bool>("IsDeleted", column => column.WithDefault(false))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}