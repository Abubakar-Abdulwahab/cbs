using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class CommandTypeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CommandType).Name,
                table => table
                            .Column<int>(nameof(CommandType.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(CommandType.Name), column => column.NotNull().Unique())
                            .Column<bool>(nameof(CommandType.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<bool>(nameof(CommandType.IsVisible), column => column.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(CommandType.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(CommandType.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(CommandType).Name);

            string queryString = string.Format("INSERT INTO {0}(" + nameof(CommandType.Name) + "," + nameof(CommandType.IsActive) + "," + nameof(CommandType.IsVisible) + "," + nameof(CommandType.CreatedAtUtc) + "," + nameof(CommandType.UpdatedAtUtc) + ") VALUES('Default', '1', '0', GETDATE(), GETDATE()), ('Tactical', '1', '1', GETDATE(), GETDATE()), ('Conventional', '1', '1', GETDATE(), GETDATE()) ", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }
    }
}