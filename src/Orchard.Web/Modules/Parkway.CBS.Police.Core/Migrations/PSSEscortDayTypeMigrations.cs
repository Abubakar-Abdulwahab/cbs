using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSEscortDayTypeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSEscortDayType).Name,
                table => table
                            .Column<int>(nameof(PSSEscortDayType.Id), column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(PSSEscortDayType.Name), column => column.NotNull().Unique())
                            .Column<bool>(nameof(PSSEscortDayType.IsActive), column => column.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(PSSEscortDayType.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PSSEscortDayType.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSEscortDayType).Name);

            string queryString = string.Format("INSERT INTO {0}(" + nameof(PSSEscortDayType.Name) + "," + nameof(PSSEscortDayType.IsActive) + "," + nameof(PSSEscortDayType.CreatedAtUtc) + "," + nameof(PSSEscortDayType.UpdatedAtUtc) + ") VALUES('Half Day', '1', GETDATE(), GETDATE()), ('Full Day', '1', GETDATE(), GETDATE())", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}