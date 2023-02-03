using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class AccountPaymentRequestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AccountPaymentRequest).Name,
                table => table
                    .Column<long>(nameof(AccountPaymentRequest.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(AccountPaymentRequest.AccountWalletConfiguration) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountPaymentRequest.FlowDefinitionLevel) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountPaymentRequest.InitiatedBy) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountPaymentRequest.Bank) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(AccountPaymentRequest.AccountName), column => column.NotNull().WithLength(200))
                    .Column<string>(nameof(AccountPaymentRequest.AccountNumber), column => column.NotNull().WithLength(20))
                    .Column<bool>(nameof(AccountPaymentRequest.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<int>(nameof(AccountPaymentRequest.PaymentRequestStatus), column => column.NotNull().WithDefault((int)PaymentRequestStatus.AWAITINGAPPROVAL))
                    .Column<DateTime>(nameof(AccountPaymentRequest.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(AccountPaymentRequest.UpdatedAtUtc), column => column.NotNull()));

            string tableName = SchemaBuilder.TableDbName(typeof(AccountPaymentRequest).Name);

            string queryString = string.Format("ALTER TABLE {0} add [{1}] as (rtrim('PMT_')+case when len(rtrim(CONVERT([nvarchar](20),[Id],0)))>(9) then CONVERT([nvarchar](20),[Id],0) else right(replicate((0),(10))+rtrim(CONVERT([nvarchar](10),[Id],0)),(10)) end) PERSISTED", tableName, nameof(AccountPaymentRequest.PaymentReference));
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}