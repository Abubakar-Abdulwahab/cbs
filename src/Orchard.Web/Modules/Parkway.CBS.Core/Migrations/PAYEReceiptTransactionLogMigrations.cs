using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEReceiptTransactionLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEReceiptTransactionLog).Name,
                table => table
                    .Column<Int64>(nameof(PAYEReceiptTransactionLog.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PAYEReceiptTransactionLog.PAYEReceipt) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PAYEReceiptTransactionLog.Receipt) + "_Id", column => column.NotNull())
                    .Column<Int64>(nameof(PAYEReceiptTransactionLog.TransactionLog) + "_Id", column => column.NotNull().Unique())
                    .Column<DateTime>(nameof(PAYEReceiptTransactionLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PAYEReceiptTransactionLog.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}