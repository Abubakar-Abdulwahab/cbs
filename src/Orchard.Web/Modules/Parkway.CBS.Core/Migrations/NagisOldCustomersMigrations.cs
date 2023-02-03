using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class NagisOldCustomersMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NagisOldCustomers).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("NagisDataBatch_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<string>("CustomerName", column => column.NotNull().WithLength(250))
                            .Column<string>("Address", column => column.NotNull().WithLength(500))
                            .Column<string>("PhoneNumber", column => column.Nullable().WithLength(50))
                            .Column<string>("CustomerId", column => column.NotNull().WithLength(50))
                            .Column<string>("TIN", column => column.Nullable().WithLength(50))
                            .Column<Int32>("TaxEntityCategory_Id", column => column.NotNull().WithLength(50))
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}