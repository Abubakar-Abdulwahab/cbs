using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class DirectAssessmentPayeeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(DirectAssessmentPayeeRecord).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("GrossAnnual", column => column.NotNull())
                            .Column<string>("Exemptions", column => column.NotNull())
                            .Column<string>("IncomeTaxPerMonth", column => column.Nullable())
                            .Column<decimal>("IncomeTaxPerMonthValue", column => column.Nullable())
                            .Column<string>("TIN", column => column.Nullable().WithLength(50))
                            .Column<string>("Month", column => column.Nullable().WithLength(11))
                            .Column<string>("Year", column => column.Nullable().WithLength(11))
                            .Column<string>("Email", column => column.Nullable().WithLength(100))
                            .Column<string>("PhoneNumber", column => column.Nullable().WithLength(20))
                            .Column<string>("PayeeName", column => column.Nullable().WithLength(200))
                            .Column<string>("Address", column => column.Nullable().WithLength(1000))
                            .Column<string>("LGA", column => column.Nullable().WithLength(100))
                            .Column<bool>("HasErrors", column => column.NotNull())                            
                            .Column<string>("ErrorMessages", column => column.Nullable().Unlimited())
                            .Column<Int64>("DirectAssessmentBatchRecord_Id", column => column.NotNull())
                            //.Column<bool>("InvoiceConfirmed", column => column.WithDefault(false))
                            //.Column<bool>("PaymentStatus", column => column.WithDefault(false))
                            .Column<DateTime>("AssessmentDate", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentPayeeRecord).Name, table => table.AddColumn("ReceiptNumber", System.Data.DbType.String));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(DirectAssessmentPayeeRecord).Name, table => table.AddColumn("EmployeeNumber", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(DirectAssessmentPayeeRecord).Name, table => table.AddColumn("GradeLevel", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(DirectAssessmentPayeeRecord).Name, table => table.AddColumn("Step", System.Data.DbType.String));
            return 3;
        }
    }
}