using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class ActivityPermissionMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ActivityPermission).Name,
                table => table
                            .Column<int>(nameof(ActivityPermission.Id), c => c.Identity().PrimaryKey())
                            .Column<Int64>(nameof(ActivityPermission.ActivityId), c => c.NotNull())
                            .Column<int>(nameof(ActivityPermission.CBSPermission) + "_Id", c => c.NotNull())
                            .Column<bool>(nameof(ActivityPermission.Value), c => c.NotNull().WithDefault(false))
                            .Column<int>(nameof(ActivityPermission.ActivityType), c => c.NotNull())
                            .Column<bool>(nameof(ActivityPermission.IsDeleted), c => c.NotNull().WithDefault(false))
                            .Column<DateTime>(nameof(ActivityPermission.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(ActivityPermission.UpdatedAtUtc), c => c.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(ActivityPermission).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ActivityPermission_Unique_Constraint UNIQUE([{1}], [{2}], [{3}]); ", tableName, nameof(ActivityPermission.CBSPermission) + "_Id", nameof(ActivityPermission.ActivityId), nameof(ActivityPermission.ActivityType)));
            return 1;
        }
    }
}