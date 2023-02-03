using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class IPPISRecordsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(IPPISBatchRecords).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("IPPISBatch_Id", column => column.NotNull())
                            .Column<string>("MinistryName", column => column.Nullable().WithLength(500))
                            .Column<string>("TaxPayerCode", column => column.Nullable().WithLength(500))
                            .Column<string>("EmployeeNumber", column => column.Nullable().WithLength(500))
                            .Column<string>("PayeeName", column => column.Nullable().WithLength(500))
                            .Column<string>("GradeLevel", column => column.Nullable().WithLength(500))
                            .Column<string>("Step", column => column.Nullable().WithLength(500))
                            .Column<string>("Address", column => column.Nullable().WithLength(1000))
                            .Column<string>("Email", column => column.Nullable().WithLength(500))
                            .Column<string>("PhoneNumber", column => column.Nullable().WithLength(500))
                            .Column<string>("TaxStringValue", column => column.Nullable().WithLength(500))
                            .Column<decimal>("Tax", column => column.Nullable())
                            .Column<bool>("HasErrors", column => column.NotNull().WithDefault(false))
                            .Column<string>("ErrorMessages", column => column.Nullable().Unlimited())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}