using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class UserNotificationMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(UserNotification).Name,
                table => table
                            .Column<long>(nameof(UserNotification.Id), column => column.PrimaryKey().Identity())
                            .Column<int>(nameof(UserNotification.MessageCount), column => column.NotNull())
                            .Column<bool>(nameof(UserNotification.IsDeleted), column => column.NotNull().WithDefault(false))
                            .Column<long>(nameof(UserNotification.User) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(UserNotification.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(UserNotification.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}