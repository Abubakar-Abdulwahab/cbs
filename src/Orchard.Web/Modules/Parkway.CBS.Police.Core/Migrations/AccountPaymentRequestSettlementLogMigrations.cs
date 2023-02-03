using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class AccountPaymentRequestSettlementLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AccountPaymentRequestSettlementLog).Name,
                table => table
                    .Column<long>(nameof(AccountPaymentRequestSettlementLog.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(AccountPaymentRequestSettlementLog.Reference), column => column.NotNull())
                    .Column<string>(nameof(AccountPaymentRequestSettlementLog.PaymentReference), column => column.NotNull().WithLength(100))
                    .Column<int>(nameof(AccountPaymentRequestSettlementLog.TransactionStatus), column => column.NotNull())
                    .Column<DateTime>(nameof(AccountPaymentRequestSettlementLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(AccountPaymentRequestSettlementLog.UpdatedAtUtc), column => column.NotNull()));

            return 1;
        }
    }
}
