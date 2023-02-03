using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class NagisOldInvoiceCustomerResponseMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NagisOldInvoiceCustomerResponse).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("BatchIdentifier", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<Int64>("PrimaryContactId", column => column.NotNull())
                            .Column<Int64>("CashflowCustomerId", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }

    }
}