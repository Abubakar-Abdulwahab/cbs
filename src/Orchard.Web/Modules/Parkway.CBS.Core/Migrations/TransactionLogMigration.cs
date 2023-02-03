using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class TransactionLogMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TransactionLog).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("Invoice_Id", column => column.NotNull())
                    .Column<decimal>("AmountPaid", column => column.NotNull())
                    .Column<DateTime>("PaymentDate", column => column.NotNull())
                    .Column<string>("PaymentReference", column => column.NotNull())
                    .Column<string>("Status", column => column.NotNull())
                    .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            


            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("TaxEntityCategory_Id", System.Data.DbType.Int32, cmd => cmd.NotNull()));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("AdminUser_Id", System.Data.DbType.Int32));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("UpdatedByAdmin", System.Data.DbType.Boolean));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("PayerName", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("InvoiceNumber", System.Data.DbType.String, cmd => cmd.NotNull()));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("PayerEmail", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("AgencyCode", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("RevenueHeadCode", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("ServiceType", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("Bank", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("BankChannel", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("RequestDump", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("Channel", System.Data.DbType.Int32));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("TransactionDate", System.Data.DbType.DateTime));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("RevenueHead_Id", System.Data.DbType.Int32, cmd => cmd.NotNull()));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("MDA_Id", System.Data.DbType.Int32, cmd => cmd.NotNull()));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("Receipt_Id", System.Data.DbType.Int32));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("ReceiptNumber", System.Data.DbType.String));

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.DropColumn("ReceiptNumber"));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.DropColumn("Receipt_Id"));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("Receipt_Id", System.Data.DbType.Int64));
            return 5;
        }
        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("ReceiptNumber", System.Data.DbType.String));
            return 6;
        }


        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("BankCode", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("BankBranch", System.Data.DbType.String));
            return 7;
        }


        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AlterColumn("BankBranch", c => c.WithType(System.Data.DbType.String).Unlimited()));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AlterColumn("RequestDump", c => c.WithType(System.Data.DbType.String).Unlimited()));
            return 8;
        }

        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("SlipNumber", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("TellerName", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("PayerPhoneNumber", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("PayerAddress", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("ItemName", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("ItemCode", System.Data.DbType.String));
            return 9;
        }

        public int UpdateFrom9()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("PaymentMethod", System.Data.DbType.String));
            return 10;
        }


        public int UpdateFrom10()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("PaymentLogId", System.Data.DbType.String, column => column.WithLength(250)));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("ThirdPartyReceiptNumber", System.Data.DbType.String, column => column.Unlimited()));
            return 11;
        }

        public int UpdateFrom11()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("RetrievalReferenceNumber", System.Data.DbType.String));
            return 12;
        }

        public int UpdateFrom12()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("TypeID", System.Data.DbType.Int32));
            return 13;
        }

        public int UpdateFrom13()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("OriginalPaymentLogID", System.Data.DbType.String, column => column.WithLength(500)));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("OriginalPaymentReference", System.Data.DbType.String, column => column.WithLength(500)));
            return 14;
        }

        public int UpdateFrom14()
        {
            return 15;
        }

        public int UpdateFrom15()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("PaymentMethodId", System.Data.DbType.Int32));
            return 16;
        }

        public int UpdateFrom16()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("PaymentProvider", System.Data.DbType.Int32));
            return 17;
        }

        public int UpdateFrom17()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("TotalAmountPaid", System.Data.DbType.Decimal));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("Fee", System.Data.DbType.Decimal));
            return 18;
        }

        public int UpdateFrom18()
        {
            return 19;
        }


        public int UpdateFrom19()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("InvoiceItem_Id", System.Data.DbType.Int64, cmd => cmd.NotNull()));
            string tableName = SchemaBuilder.TableDbName(typeof(TransactionLog).Name);
            string queryString = string.Format("ALTER TABLE {0} add [CompositeUniquePaymentReference] as ((case when [TypeID] = (3) then concat([PaymentProvider],'-',[PaymentReference],'-',CONVERT([nvarchar](50),[InvoiceItem_Id])) else CONVERT([nvarchar](50),[Id]) end)) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            string unqiueQuery = string.Format("ALTER TABLE[dbo].[{0}] ADD constraint PaymentReference_Unique_Constraint UNIQUE([CompositeUniquePaymentReference]); ", tableName);
            SchemaBuilder.ExecuteSql(unqiueQuery);
            return 20;
        }


        public int UpdateFrom20()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("Settled", System.Data.DbType.Boolean));
            return 21;
        }


        public int UpdateFrom21()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn("Reversed", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 22;
        }


        public int UpdateFrom22()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn(nameof(TransactionLog.SettlementFeeDeduction), System.Data.DbType.Decimal, column => column.WithDefault(0.00m)));

            string tableName = SchemaBuilder.TableDbName(typeof(TransactionLog).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = {2}", tableName, nameof(TransactionLog.SettlementFeeDeduction), 0.00m);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} decimal(19, 5) NOT NULL", tableName, nameof(TransactionLog.SettlementFeeDeduction));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} add [{1}] as (([{2}]-[{3}])) ", tableName, nameof(TransactionLog.SettlementAmount), nameof(TransactionLog.AmountPaid), nameof(TransactionLog.SettlementFeeDeduction));
            SchemaBuilder.ExecuteSql(queryString);

            return 23;
        }


        public int UpdateFrom23()
        {
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn(nameof(TransactionLog.SettlementDate), System.Data.DbType.DateTime));
            SchemaBuilder.AlterTable(typeof(TransactionLog).Name, table => table.AddColumn(nameof(TransactionLog.SettlmentBatchIdentifier), System.Data.DbType.Int64));
            return 24;
        }

    }
}