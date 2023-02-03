using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceRequestFlowApproverMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceRequestFlowApprover).Name,
                table => table
                    .Column<int>(nameof(PSServiceRequestFlowApprover.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSServiceRequestFlowApprover.FlowDefinitionLevel) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSServiceRequestFlowApprover.AssignedApprover) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSServiceRequestFlowApprover.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSServiceRequestFlowApprover.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowApprover).Name, table => table.DropColumn("ContactEmail"));
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowApprover).Name, table => table.DropColumn("ContactPhoneNumber"));
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowApprover).Name, table => table.AddColumn(nameof(PSServiceRequestFlowApprover.PSSAdminUser) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 2;
        }

        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSServiceRequestFlowApprover).Name);

            string queryString = string.Format("UPDATE {0} SET [PSSAdminUser_Id] = 1", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN PSSAdminUser_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PSServiceRequestFlowApprover).Name, table => table.AddColumn("IsDeleted", System.Data.DbType.Boolean, column => column.NotNull().WithDefault(false)));
            return 4;
        }

    }
}