using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class RequestStatusLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RequestStatusLog).Name,
                table => table
                    .Column<Int64>(nameof(RequestStatusLog.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(RequestStatusLog.Request)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(RequestStatusLog.StatusDescription), column => column.NotNull().WithLength(500))
                    .Column<int>(nameof(RequestStatusLog.Status), column => column.NotNull())
                    .Column<bool>("HasInvoice", column => column.NotNull())
                    .Column<Int64>(nameof(RequestStatusLog.Invoice) + "_Id", column => column.Nullable())
                    .Column<int>(nameof(RequestStatusLog.FlowDefinitionLevel) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(RequestStatusLog.UserActionRequired), column => column.NotNull())
                    .Column<bool>(nameof(RequestStatusLog.Fulfilled), column => column.NotNull().WithDefault(false))
                    .Column<DateTime>(nameof(RequestStatusLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(RequestStatusLog.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(RequestStatusLog).Name, table => table.DropColumn("HasInvoice"));
            //
            string tableName = SchemaBuilder.TableDbName(typeof(RequestStatusLog).Name);

            string queryString = string.Format("UPDATE r SET r.{1} = innerR.{1} FROM {0} r INNER JOIN( SELECT {1}, {2} FROM {0} WHERE {1} IS NOT NULL) innerR ON innerR.Request_Id = r.Request_Id AND r.Invoice_Id IS NULL", tableName, nameof(RequestStatusLog.Invoice) + "_Id", nameof(RequestStatusLog.Request) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} bigint NOT NULL", tableName, nameof(RequestStatusLog.Invoice) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 2;
        }
        
    }
}