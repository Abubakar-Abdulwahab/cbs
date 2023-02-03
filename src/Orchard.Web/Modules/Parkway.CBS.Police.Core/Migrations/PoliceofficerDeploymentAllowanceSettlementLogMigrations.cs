using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceofficerDeploymentAllowanceSettlementLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceofficerDeploymentAllowanceSettlementLog).Name,
                table => table
                    .Column<long>(nameof(PoliceofficerDeploymentAllowanceSettlementLog.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PoliceofficerDeploymentAllowanceSettlementLog.ItemReference), column => column.NotNull())
                    .Column<string>(nameof(PoliceofficerDeploymentAllowanceSettlementLog.ReferenceNumber), column => column.NotNull().WithLength(100))
                    .Column<int>(nameof(PoliceofficerDeploymentAllowanceSettlementLog.TransactionStatus), column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceofficerDeploymentAllowanceSettlementLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceofficerDeploymentAllowanceSettlementLog.UpdatedAtUtc), column => column.NotNull()));

            string tableName = SchemaBuilder.TableDbName(typeof(PoliceofficerDeploymentAllowanceSettlementLog).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint DeploymentAllowanceSettlementLog_Unique_Reference_ItemReference_ReferenceNumber_TransactionStatus_Constraint UNIQUE([{1}], [{2}], [{3}]); ", tableName, nameof(PoliceofficerDeploymentAllowanceSettlementLog.ItemReference), nameof(PoliceofficerDeploymentAllowanceSettlementLog.ReferenceNumber), nameof(PoliceofficerDeploymentAllowanceSettlementLog.TransactionStatus)));

            return 1;
        }

    }
}