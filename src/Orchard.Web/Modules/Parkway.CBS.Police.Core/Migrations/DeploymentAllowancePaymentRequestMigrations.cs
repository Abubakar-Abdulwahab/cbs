using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class DeploymentAllowancePaymentRequestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(DeploymentAllowancePaymentRequest).Name,
                table => table
                    .Column<long>(nameof(DeploymentAllowancePaymentRequest.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(DeploymentAllowancePaymentRequest.AccountWalletConfiguration) + "_Id", column => column.NotNull())
                    .Column<long>(nameof(DeploymentAllowancePaymentRequest.PSSRequestInvoice) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(DeploymentAllowancePaymentRequest.FlowDefinitionLevel) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(DeploymentAllowancePaymentRequest.InitiatedBy) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(DeploymentAllowancePaymentRequest.Bank) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(DeploymentAllowancePaymentRequest.AccountName), column => column.NotNull().WithLength(200))
                    .Column<string>(nameof(DeploymentAllowancePaymentRequest.AccountNumber), column => column.NotNull().WithLength(20))
                    .Column<bool>(nameof(DeploymentAllowancePaymentRequest.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<int>(nameof(DeploymentAllowancePaymentRequest.PaymentRequestStatus), column => column.NotNull().WithDefault((int)PaymentRequestStatus.AWAITINGAPPROVAL))
                    .Column<DateTime>(nameof(DeploymentAllowancePaymentRequest.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(DeploymentAllowancePaymentRequest.UpdatedAtUtc), column => column.NotNull()));

            string tableName = SchemaBuilder.TableDbName(typeof(DeploymentAllowancePaymentRequest).Name);

            string queryString = string.Format("ALTER TABLE {0} add [{1}] as (rtrim('DEPALW_')+case when len(rtrim(CONVERT([nvarchar](20),[Id],0)))>(9) then CONVERT([nvarchar](20),[Id],0) else right(replicate((0),(10))+rtrim(CONVERT([nvarchar](10),[Id],0)),(10)) end) PERSISTED", tableName, nameof(DeploymentAllowancePaymentRequest.PaymentReference));
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}