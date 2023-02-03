using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class IPPISTaxPayerSummaryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(IPPISTaxPayerSummary).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("IPPISBatch_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<int>("TaxEntityCategory_Id", column => column.Nullable())
                            .Column<string>("TaxPayerCode", column => column.NotNull().WithLength(100))
                            .Column<int>("NumberofEmployees")
                            .Column<decimal>("TotalTaxAmount", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}