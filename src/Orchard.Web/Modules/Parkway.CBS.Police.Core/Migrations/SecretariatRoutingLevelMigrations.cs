using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class SecretariatRoutingLevelMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(SecretariatRoutingLevel).Name,
                table => table
                    .Column<Int64>(nameof(SecretariatRoutingLevel.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(SecretariatRoutingLevel.Request) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(SecretariatRoutingLevel.StageRoutedTo), column => column.NotNull())
                    .Column<string>(nameof(SecretariatRoutingLevel.StageModelName), column => column.NotNull())
                    .Column<int>(nameof(SecretariatRoutingLevel.AdminUser)+"_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(SecretariatRoutingLevel.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(SecretariatRoutingLevel.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(SecretariatRoutingLevel).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint SecretariatRoutingLevel_Unique_Constraint UNIQUE([{1}], [{2}], [{3}]); ", tableName, nameof(SecretariatRoutingLevel.Request) + "_Id", nameof(SecretariatRoutingLevel.StageRoutedTo), nameof(SecretariatRoutingLevel.StageModelName)));

            return 1;
        }

    }
}
