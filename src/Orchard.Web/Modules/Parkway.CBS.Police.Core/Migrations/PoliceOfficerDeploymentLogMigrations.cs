using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceOfficerDeploymentLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceOfficerDeploymentLog).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("OfficerName", column => column.NotNull().WithLength(500))
                    .Column<string>("Address", column => column.NotNull().WithLength(500))
                    .Column<string>("CustomerName", column => column.NotNull().WithLength(500))
                    .Column<Int64>("Request_Id", column => column.NotNull())
                    .Column<DateTime>("StartDate", column => column.NotNull())
                    .Column<DateTime>("EndDate", column => column.NotNull())
                    .Column<Int64>("PoliceOfficer_Id", column => column.NotNull())
                    .Column<int>("Command_Id", column => column.NotNull())
                    .Column<int>("State_Id", column => column.NotNull())
                    .Column<int>("LGA_Id", column => column.NotNull())
                    .Column<Int64>("Invoice_Id", column => column.NotNull())
                    .Column<Int64>("OfficerRank_Id", column => column.NotNull())
                    .Column<bool>("IsActive", column => column.NotNull())
                    .Column<int>("Status", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PoliceOfficerDeploymentLog).Name, table => table.AddColumn("RelievingOfficer_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PoliceOfficerDeploymentLog).Name, table => table.AddColumn(nameof(PoliceOfficerDeploymentLog.DeploymentRate), System.Data.DbType.Decimal, column => column.WithDefault(0)));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PoliceOfficerDeploymentLog).Name, table => table.AddColumn(nameof(PoliceOfficerDeploymentLog.DeploymentEndReason), System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PoliceOfficerDeploymentLog).Name, table => table.AddColumn(nameof(PoliceOfficerDeploymentLog.DeploymentEndBy) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));

            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(PoliceOfficerDeploymentLog).Name, table => table.AddColumn(nameof(PoliceOfficerDeploymentLog.PoliceOfficerLog)+"_Id", System.Data.DbType.Int64, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PoliceOfficerDeploymentLog).Name, table => table.AddColumn(nameof(PoliceOfficerDeploymentLog.RelievingOfficerLog) + "_Id", System.Data.DbType.Int64, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PoliceOfficerDeploymentLog).Name);
            string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN PoliceOfficer_Id bigint NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 5;
        }
    }
}