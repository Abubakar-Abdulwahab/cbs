using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class IdentificationTypeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(IdentificationType).Name,
                table => table
                    .Column<int>(nameof(IdentificationType.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(IdentificationType.Name), column => column.NotNull().Unique())
                    .Column<bool>(nameof(IdentificationType.HasIntegration), column => column.NotNull())
                    .Column<string>(nameof(IdentificationType.ImplementationClass), column => column.Nullable())
                    .Column<string>(nameof(IdentificationType.ImplementingClassName), column => column.Nullable())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            string tableName = SchemaBuilder.TableDbName(typeof(IdentificationType).Name);
            string queryString = string.Format("CREATE UNIQUE INDEX CHECK_FOR_DUPLICATE_IMPLEMENTING_CLASS_NAME_IF_NOT_NULL ON [dbo].[{0}](ImplementingClassName) WHERE ImplementingClassName IS NOT NULL ", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(IdentificationType).Name, table => table.AddColumn(nameof(IdentificationType.Hint), System.Data.DbType.String, column => column.Nullable()));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(IdentificationType).Name, table => table.AddColumn(nameof(IdentificationType.HasBiometricSupport), System.Data.DbType.Boolean, column => column.Nullable()));

            SchemaBuilder.ExecuteSql(string.Format("UPDATE {0} SET [{1}] = 0", SchemaBuilder.TableDbName(typeof(IdentificationType).Name), nameof(IdentificationType.HasBiometricSupport)));

            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE {0} ALTER COLUMN {1} bit NOT NULL", SchemaBuilder.TableDbName(typeof(IdentificationType).Name), nameof(IdentificationType.HasBiometricSupport)));

            return 3;
        }
    }
}