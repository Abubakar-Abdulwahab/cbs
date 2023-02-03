using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class NibssIntegrationCredentialMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NibssIntegrationCredentials).Name,
                table => table
                            .Column<int>(nameof(NibssIntegrationCredentials.Id), c => c.Identity().PrimaryKey())
                            .Column<string>(nameof(NibssIntegrationCredentials.IV), c => c.NotNull())
                            .Column<string>(nameof(NibssIntegrationCredentials.SecretKey), c => c.NotNull())
                            .Column<bool>(nameof(NibssIntegrationCredentials.IsActive), c => c.NotNull().WithDefault(true))
                            .Column<DateTime>(nameof(NibssIntegrationCredentials.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(NibssIntegrationCredentials.UpdatedAtUtc), c => c.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(NibssIntegrationCredentials).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint NibssIntegrationCredentials_IV_Unique_Constraint UNIQUE([{1}]); ", tableName, nameof(NibssIntegrationCredentials.IV)));
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint NibssIntegrationCredentials_SecretKey_Unique_Constraint UNIQUE([{1}]); ", tableName, nameof(NibssIntegrationCredentials.SecretKey)));
            return 1;
        }
    }
}