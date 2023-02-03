using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class UserNotificationItemMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(UserNotificationItem).Name,
                table => table
                            .Column<long>(nameof(UserNotificationItem.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(UserNotificationItem.Message), column => column.NotNull().Unlimited())
                            .Column<bool>(nameof(UserNotificationItem.IsDeleted), column => column.NotNull().WithDefault(false))
                            .Column<bool>(nameof(UserNotificationItem.IsRead), column => column.NotNull().WithDefault(false))
                            .Column<long>(nameof(UserNotificationItem.UserNotification) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(UserNotificationItem.ReadAt), column => column.Nullable())
                            .Column<DateTime>(nameof(UserNotificationItem.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(UserNotificationItem.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}