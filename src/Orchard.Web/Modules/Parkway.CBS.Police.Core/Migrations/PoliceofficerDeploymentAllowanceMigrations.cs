using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceofficerDeploymentAllowanceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceofficerDeploymentAllowance).Name,
                table => table
                    .Column<Int64>(nameof(PoliceofficerDeploymentAllowance.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PoliceofficerDeploymentAllowance.Status), column => column.NotNull())
                    .Column<string>(nameof(PoliceofficerDeploymentAllowance.Comment), column => column.Nullable())
                    .Column<int>(nameof(PoliceofficerDeploymentAllowance.PoliceOfficer) + "_Id", column => column.NotNull())
                    .Column<decimal>(nameof(PoliceofficerDeploymentAllowance.Amount), column => column.NotNull())
                    .Column<int>(nameof(PoliceofficerDeploymentAllowance.InitiatedBy) + "_Id", column => column.Nullable())
                    .Column<Int64>(nameof(PoliceofficerDeploymentAllowance.EscortDetails) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PoliceofficerDeploymentAllowance.Narration), column => column.NotNull())
                    .Column<string>(nameof(PoliceofficerDeploymentAllowance.SettlementReferenceNumber), column => column.Nullable())
                    .Column<DateTime>(nameof(PoliceofficerDeploymentAllowance.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceofficerDeploymentAllowance.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PoliceofficerDeploymentAllowance).Name, table => table.CreateIndex("SettlementReferenceNumberIndex", new string[] { nameof(PoliceofficerDeploymentAllowance.SettlementReferenceNumber) }));
            SchemaBuilder.AlterTable(typeof(PoliceofficerDeploymentAllowance).Name, table => table.AddColumn(nameof(PoliceofficerDeploymentAllowance.ContributedAmount), System.Data.DbType.Decimal, column => column.NotNull()));
            SchemaBuilder.AlterTable(typeof(PoliceofficerDeploymentAllowance).Name, table => table.AddColumn(nameof(PoliceofficerDeploymentAllowance.Request) + "_Id", System.Data.DbType.Int64, column => column.NotNull()));
            SchemaBuilder.AlterTable(typeof(PoliceofficerDeploymentAllowance).Name, table => table.AddColumn(nameof(PoliceofficerDeploymentAllowance.Invoice) + "_Id", System.Data.DbType.Int64, column => column.NotNull()));
            SchemaBuilder.AlterTable(typeof(PoliceofficerDeploymentAllowance).Name, table => table.AddColumn(nameof(PoliceofficerDeploymentAllowance.ScheduleJob_Id), System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PoliceofficerDeploymentAllowance).Name, table => table.AddColumn(nameof(PoliceofficerDeploymentAllowance.PaymentStage), System.Data.DbType.Int32, column => column.NotNull()));
            return 2;
        }

        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PoliceofficerDeploymentAllowance).Name);
            SchemaBuilder.AlterTable(typeof(PoliceofficerDeploymentAllowance).Name, table => table.AddColumn(nameof(PoliceofficerDeploymentAllowance.Command) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));

            string queryString = string.Format("UPDATE {0} SET [Command_Id] = (select top 1 Id from Parkway_CBS_Police_Core_Command)", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN Command_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PoliceofficerDeploymentAllowance).Name, table => table.AddColumn(nameof(PoliceofficerDeploymentAllowance.PoliceOfficerLog) + "_Id", System.Data.DbType.Int64, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PoliceofficerDeploymentAllowance).Name);
            string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN PoliceOfficer_Id int NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 4;
        }

    }
}