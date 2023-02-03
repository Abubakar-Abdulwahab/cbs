using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class WebPaymentRequestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(WebPaymentRequest).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<decimal>("Amount", column => column.NotNull())
                    .Column<string>("InvoiceNumber", column => column.NotNull().WithLength(100))
                    .Column<string>("TransactionReference", column => column.NotNull().Unique().WithLength(100))
                    .Column<string>("CallBackURL", column => column.Nullable().WithLength(250))
                    .Column<decimal>("FeeApplied", column => column.NotNull())
                    .Column<string>("ClientId", column => column.Nullable().WithLength(255))
                    .Column<string>("RequestIdentifier", column => column.Nullable().WithLength(100))
                    .Column<string>("RequestDump", column => column.Nullable().Unlimited())
                    .Column<string>("ResponseDump", column => column.Nullable().Unlimited())
                    .Column<string>("ResponseCode", column => column.Nullable().WithLength(10))
                    .Column<string>("RequestSource", column => column.NotNull().WithLength(50))
                    .Column<string>("WebPaymentChannel", column => column.NotNull().WithLength(50))
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}