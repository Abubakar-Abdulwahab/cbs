using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRequestApprovalLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRequestApprovalLog).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("Request_Id", column => column.NotNull())
                    .Column<int>("Status", column => column.NotNull())
                    .Column<int>("FlowDefinitionLevel_Id", column => column.NotNull())
                    .Column<int>("AddedByAdminUser_Id", column => column.NotNull())
                    .Column<string>("Comment", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PSSRequestApprovalLog).Name);

            string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} nvarchar(1000) NOT NULL", tableName,nameof(PSSRequestApprovalLog.Comment));
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }

    }
}