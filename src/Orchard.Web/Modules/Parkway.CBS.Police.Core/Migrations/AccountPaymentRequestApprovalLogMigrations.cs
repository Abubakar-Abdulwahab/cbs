using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class AccountPaymentRequestApprovalLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AccountPaymentRequestApprovalLog).Name,
                table => table
                    .Column<long>(nameof(AccountPaymentRequestApprovalLog.Id), column => column.PrimaryKey().Identity())
                    .Column<long>(nameof(AccountPaymentRequestApprovalLog.PaymentRequest) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountPaymentRequestApprovalLog.FlowDefinitionLevel) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountPaymentRequestApprovalLog.AddedByAdminUser) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountPaymentRequestApprovalLog.Status), column => column.NotNull())
                    .Column<string>(nameof(AccountPaymentRequestApprovalLog.Comment), column => column.NotNull().WithLength(1000))
                    .Column<DateTime>(nameof(AccountPaymentRequestApprovalLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(AccountPaymentRequestApprovalLog.UpdatedAtUtc), column => column.NotNull()));

            return 1;
        }
    }
}