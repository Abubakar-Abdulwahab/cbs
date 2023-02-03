using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBatchItemsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBatchItems).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<decimal>("GrossAnnual", column => column.NotNull())
                            .Column<decimal>("Exemptions", column => column.NotNull())
                            .Column<decimal>("IncomeTaxPerMonth", column => column.Nullable())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<Int64>("PAYEBatchRecord_Id", column => column.NotNull())
                            .Column<int>("Month", column => column.NotNull())
                            .Column<int>("Year", column => column.NotNull())
                            .Column<DateTime>("AssessmentDate", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}