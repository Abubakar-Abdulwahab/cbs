using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class AccountWalletConfigurationMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AccountWalletConfiguration).Name,
                table => table
                    .Column<int>(nameof(AccountWalletConfiguration.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(AccountWalletConfiguration.CommandWalletDetail) + "_Id", column => column.Nullable())
                    .Column<int>(nameof(AccountWalletConfiguration.PSSFeeParty) + "_Id", column => column.Nullable())
                    .Column<int>(nameof(AccountWalletConfiguration.FlowDefinition) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(AccountWalletConfiguration.LastUpdatedBy) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(AccountWalletConfiguration.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(AccountWalletConfiguration.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(AccountWalletConfiguration.UpdatedAtUtc), column => column.NotNull()));

            string tableName = SchemaBuilder.TableDbName(typeof(AccountWalletConfiguration).Name);

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE [dbo].[{0}] ADD constraint CommandWalletDetails_FlowDefinition_Unique_Constraint UNIQUE([{1}], [{2}]);", tableName, nameof(AccountWalletConfiguration.CommandWalletDetail) + "_Id", nameof(AccountWalletConfiguration.FlowDefinition) + "_Id"));

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE [dbo].[{0}] ADD constraint PSSFeeParty_FlowDefinition_Unique_Constraint UNIQUE([{1}], [{2}]);", tableName, nameof(AccountWalletConfiguration.PSSFeeParty) + "_Id", nameof(AccountWalletConfiguration.FlowDefinition) + "_Id"));


            return 1;
        }
    }
}