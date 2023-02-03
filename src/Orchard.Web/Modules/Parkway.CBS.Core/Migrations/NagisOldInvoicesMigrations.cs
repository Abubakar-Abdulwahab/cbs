using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class NagisOldInvoicesMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NagisOldInvoices).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("NagisDataBatch_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<int>("RevenueHead_Id", column => column.NotNull().WithLength(20))
                            .Column<int>("TaxEntityCategory_Id", column => column.NotNull())
                            .Column<int>("NagisOldInvoiceSummary_Id", column => column.Nullable())
                            .Column<int>("OperationType_Id", column => column.NotNull())
                            .Column<string>("CustomerName", column => column.NotNull().WithLength(250))
                            .Column<string>("Address", column => column.NotNull().WithLength(500))
                            .Column<string>("PhoneNumber", column => column.Nullable().WithLength(50))
                            .Column<string>("CustomerId", column => column.NotNull().WithLength(50))
                            .Column<decimal>("Amount", column => column.NotNull())
                            .Column<string>("TIN", column => column.Nullable().WithLength(50))
                            .Column<string>("NagisInvoiceNumber", column => column.NotNull().WithLength(50))
                            .Column<DateTime>("NagisInvoiceCreationDate", column => column.NotNull())
                            .Column<string>("ExternalRefId", column => column.NotNull().WithLength(250))
                            .Column<string>("InvoiceDescription", column => column.Nullable().WithLength(500))
                            .Column<decimal>("AmountDue", column => column.NotNull())
                            .Column<int>("Quantity", column => column.NotNull())
                            .Column<int>("Status", column => column.NotNull())
                            .Column<int>("GroupId", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}