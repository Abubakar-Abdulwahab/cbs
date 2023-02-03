using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class AccountPaymentRequestItemsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AccountPaymentRequestItem).Name,
                table => table
                    .Column<long>(nameof(AccountPaymentRequestItem.Id), column => column.PrimaryKey().Identity())
                    .Column<long>(nameof(AccountPaymentRequestItem.AccountPaymentRequest) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountPaymentRequestItem.PSSExpenditureHead) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountPaymentRequestItem.Bank) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(AccountPaymentRequestItem.AccountNumber), column => column.NotNull().WithLength(12))
                    .Column<string>(nameof(AccountPaymentRequestItem.AccountName), column => column.NotNull().WithLength(100))
                    .Column<decimal>(nameof(AccountPaymentRequestItem.Amount), column => column.NotNull())
                    .Column<string>(nameof(AccountPaymentRequestItem.BeneficiaryName), column => column.NotNull().WithLength(100))
                    .Column<string>(nameof(AccountPaymentRequestItem.PaymentReference), column => column.NotNull().WithLength(100))
                    .Column<int>(nameof(AccountPaymentRequestItem.TransactionStatus), column => column.NotNull().WithDefault((int)PaymentRequestStatus.AWAITINGAPPROVAL))
                    .Column<DateTime>(nameof(AccountPaymentRequestItem.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(AccountPaymentRequestItem.UpdatedAtUtc), column => column.NotNull()));

            return 1;
        }
    }
}
