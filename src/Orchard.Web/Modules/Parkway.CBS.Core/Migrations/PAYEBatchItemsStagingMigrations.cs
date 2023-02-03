using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBatchItemsStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBatchItemsStaging).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("GrossAnnual", column => column.Nullable())
                            .Column<decimal>("GrossAnnualValue", column => column.Nullable())
                            .Column<string>("Exemptions", column => column.Nullable())
                            .Column<decimal>("ExemptionsValue", column => column.Nullable())
                            .Column<string>("IncomeTaxPerMonth", column => column.Nullable())
                            .Column<decimal>("IncomeTaxPerMonthValue", column => column.Nullable())
                            .Column<string>("PayerId", column => column.Nullable())
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<Int64>("PAYEBatchRecordStaging_Id", column => column.NotNull())
                            .Column<string>("Month", column => column.Nullable().WithLength(11))
                            .Column<int>("MonthValue", column => column.Nullable())
                            .Column<string>("Year", column => column.Nullable().WithLength(11))
                            .Column<int>("YearValue", column => column.Nullable())
                            .Column<bool>("HasErrors", column => column.NotNull())
                            .Column<string>("ErrorMessages", column => column.Nullable().Unlimited())
                            .Column<DateTime>("AssessmentDate", column => column.Nullable())
                            .Column<string>("ReceiptNumber", column => column.Nullable())
                            .Column<string>("EmployeeNumber", column => column.Nullable())
                            .Column<string>("GradeLevel", column => column.Nullable())
                            .Column<string>("Step", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PAYEBatchItemsStaging).Name, table => table.AddColumn("SerialNumber", System.Data.DbType.Int32, column => column.NotNull()));
            return 2;
        }

    }
}