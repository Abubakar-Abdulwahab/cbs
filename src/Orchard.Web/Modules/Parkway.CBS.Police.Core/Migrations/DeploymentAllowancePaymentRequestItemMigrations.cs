using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class DeploymentAllowancePaymentRequestItemMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(DeploymentAllowancePaymentRequestItem).Name,
                table => table
                    .Column<long>(nameof(DeploymentAllowancePaymentRequestItem.Id), column => column.PrimaryKey().Identity())
                    .Column<long>(nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(DeploymentAllowancePaymentRequestItem.Bank) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(DeploymentAllowancePaymentRequestItem.AccountName), column => column.NotNull())
                    .Column<string>(nameof(DeploymentAllowancePaymentRequestItem.AccountNumber), column => column.NotNull().WithLength(10))
                    .Column<decimal>(nameof(DeploymentAllowancePaymentRequestItem.Amount), column => column.NotNull())
                    .Column<string>(nameof(DeploymentAllowancePaymentRequestItem.PaymentReference), column => column.NotNull().WithLength(100))
                    .Column<int>(nameof(DeploymentAllowancePaymentRequestItem.TransactionStatus), column => column.NotNull().WithDefault((int)PaymentRequestStatus.AWAITINGAPPROVAL))
                    .Column<int>(nameof(DeploymentAllowancePaymentRequestItem.CommandType)+"_Id", column => column.NotNull())
                    .Column<int>(nameof(DeploymentAllowancePaymentRequestItem.DayType)+"_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(DeploymentAllowancePaymentRequestItem.StartDate), column => column.NotNull())
                    .Column<DateTime>(nameof(DeploymentAllowancePaymentRequestItem.EndDate), column => column.NotNull())
                    .Column<DateTime>(nameof(DeploymentAllowancePaymentRequestItem.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(DeploymentAllowancePaymentRequestItem.UpdatedAtUtc), column => column.NotNull()));

            string tableName = SchemaBuilder.TableDbName(typeof(DeploymentAllowancePaymentRequestItem).Name);
            string queryString = string.Format("ALTER TABLE[dbo].[{0}] ADD constraint DEPLOYMENT_ALLOWANCE_PAYMENT_REQUEST_ITEM_UNIQUE_CONSTRAINT UNIQUE([{1}], [{2}], [{3}])", tableName, nameof(DeploymentAllowancePaymentRequestItem.DeploymentAllowancePaymentRequest) + "_Id", nameof(DeploymentAllowancePaymentRequestItem.CommandType) + "_Id", nameof(DeploymentAllowancePaymentRequestItem.DayType) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}