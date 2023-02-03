using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class AccountPaymentRequestSettlementDetailMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AccountPaymentRequestSettlementDetail).Name,
                table => table
                    .Column<long>(nameof(AccountPaymentRequestSettlementDetail.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(AccountPaymentRequestSettlementDetail.SettlementEngineRequestJSON), column => column.NotNull().WithLength(4000))
                    .Column<string>(nameof(AccountPaymentRequestSettlementDetail.SettlementEngineResponseJSON), column => column.Nullable().WithLength(4000))
                    .Column<string>(nameof(AccountPaymentRequestSettlementDetail.PaymentReference), column => column.NotNull().WithLength(100))
                    .Column<DateTime>(nameof(AccountPaymentRequestSettlementDetail.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(AccountPaymentRequestSettlementDetail.UpdatedAtUtc), column => column.NotNull()));

            return 1;
        }
    }
}