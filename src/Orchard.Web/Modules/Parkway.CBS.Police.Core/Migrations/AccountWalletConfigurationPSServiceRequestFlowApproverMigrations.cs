using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class AccountWalletConfigurationPSServiceRequestFlowApproverMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AccountWalletConfigurationPSServiceRequestFlowApprover).Name,
                table => table
                    .Column<int>(nameof(AccountWalletConfigurationPSServiceRequestFlowApprover.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(AccountWalletConfigurationPSServiceRequestFlowApprover.PSServiceRequestFlowApprover) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountWalletConfigurationPSServiceRequestFlowApprover.AccountWalletConfiguration) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(AccountWalletConfigurationPSServiceRequestFlowApprover.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(AccountWalletConfigurationPSServiceRequestFlowApprover.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(AccountWalletConfigurationPSServiceRequestFlowApprover.UpdatedAtUtc), column => column.NotNull()));

            return 1;
        }
    }
}