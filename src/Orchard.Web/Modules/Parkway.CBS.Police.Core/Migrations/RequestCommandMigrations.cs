using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class RequestCommandMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RequestCommand).Name,
                table => table
                    .Column<Int64>(nameof(RequestCommand.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(RequestCommand.Request) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(RequestCommand.Command) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(RequestCommand.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(RequestCommand.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(RequestCommand).Name);

            SchemaBuilder.ExecuteSql(string.Format("INSERT INTO [dbo].[{0}] ({1}, {2}, {3}, {4}) SELECT {5}, {6}, {7}, {8} FROM [dbo].[{9}]", tableName, nameof(RequestCommand.Request) + "_Id", nameof(RequestCommand.Command) + "_Id", nameof(RequestCommand.CreatedAtUtc), nameof(RequestCommand.UpdatedAtUtc), nameof(PSSRequest.Id), nameof(RequestCommand.Command) + "_Id", nameof(PSSRequest.CreatedAtUtc), nameof(PSSRequest.UpdatedAtUtc), SchemaBuilder.TableDbName(typeof(PSSRequest).Name)));

            return 1;
        }
    }
}