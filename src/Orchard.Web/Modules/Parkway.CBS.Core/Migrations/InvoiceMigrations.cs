using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class InvoiceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(Invoice).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("InvoiceNumber", column => column.NotNull().Unique())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<decimal>("Amount", column => column.NotNull())
                            .Column<Int64>("TaxPayer_Id", column => column.NotNull())
                            .Column<int>("TaxPayerCategory_Id", column => column.NotNull())
                            .Column<Int64>("DirectAssessmentBatchRecord_Id", column => column.Nullable())
                            .Column<int>("ExpertSystemSettings_Id", column => column.NotNull())
                            .Column<string>("InvoiceURL", column => column.NotNull().Unlimited())
                            .Column<string>("CashflowInvoiceIdentifier", column => column.Nullable().WithLength(100))
                            .Column<string>("InvoiceModel", column => column.Nullable().Unlimited())
                            .Column<int>("Status", column => column.NotNull())
                            .Column<DateTime>("PaymentDate", column => column.Nullable())
                            .Column<DateTime>("DueDate", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(Invoice).Name, table => table.CreateIndex("CreatedAtUtc", new string[] { "CreatedAtUtc" }))
                .AlterTable(typeof(Invoice).Name, table => table.CreateIndex("SearchByInvoiceNumber", new string[] { "InvoiceNumber" }));
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("ExternalRefNumber", System.Data.DbType.String));
            return 2;
        }


        public int UpdateFrom2()
        {
            //-- this function would take in a decimal value, if the decimal value is less than 0 it would return 0, else it would return the parameter value
            string queryString = string.Format("CREATE FUNCTION dbo.GetAmountDue (@amountDue  decimal(19, 5)) RETURNS decimal(19, 5) AS BEGIN RETURN(case when @amountDue < 0 then 0 else @amountDue end); END; ");
            SchemaBuilder.ExecuteSql(queryString);

            string invoiceTableName = SchemaBuilder.TableDbName(typeof(Invoice).Name);
            string amountSummaryComputedColumn = string.Format("ALTER TABLE {0} add [InvoiceAmountDueSummary_Id] as (([Id]))", invoiceTableName);
            SchemaBuilder.ExecuteSql(amountSummaryComputedColumn);

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("GeneratedByAdminUser_Id", System.Data.DbType.Int32));
            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("InvoiceDescription", System.Data.DbType.String, cmd => cmd.NotNull()));
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("CallBackURL", System.Data.DbType.String, column => column.Unlimited()));
            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("APIRequest_Id", System.Data.DbType.Int64));
            return 7;
        }

        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("InvoiceType", System.Data.DbType.Int32, col => col.WithDefault(0)));
            return 8;
        }

        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("InvoiceTypeId", System.Data.DbType.Int64));
            return 9;
        }

        public int UpdateFrom9()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("Quantity", System.Data.DbType.Int32, cmd => cmd.NotNull()));
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("InvoiceTitle", System.Data.DbType.String, c => c.WithLength(500).NotNull()));
            return 10;
        }

        public int UpdateFrom10()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.DropColumn("DirectAssessmentBatchRecord_Id"));
            return 11;
        }

        public int UpdateFrom11()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("NAGISInvoiceNumber", System.Data.DbType.String, c => c.WithLength(50)));
            return 12;
        }

        public int UpdateFrom12()
        {
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("IsCancel", System.Data.DbType.Boolean, c => c.WithDefault(false)));
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("CancelDate", System.Data.DbType.DateTime));
            SchemaBuilder.AlterTable(typeof(Invoice).Name, table => table.AddColumn("CancelBy_Id", System.Data.DbType.Int32));
            return 13;
        }

    }
}