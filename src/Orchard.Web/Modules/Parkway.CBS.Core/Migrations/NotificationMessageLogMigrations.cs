using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class NotificationMessageLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NotificationMessageLog).Name,
                table => table
                            .Column<Int64>(nameof(NotificationMessageLog.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(NotificationMessageLog.NotificationType), column => column.NotNull())
                            .Column<string>(nameof(NotificationMessageLog.Reference), column => column.NotNull())
                            .Column<DateTime>(nameof(NotificationMessageLog.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(NotificationMessageLog.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(NotificationMessageLog).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint NotificationMessageLog_Unique_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(NotificationMessageLog.NotificationType), nameof(NotificationMessageLog.Reference)));

            return 1;
        }
    }
}