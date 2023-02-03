using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSEscortSettingsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSEscortSettings).Name,
                table => table
                    .Column<int>(nameof(PSSEscortSettings.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSSEscortSettings.AdminToAssignOfficers) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSEscortSettings.WorkFlowDefinition) + "_Id", column => column.NotNull().Unique())
                    .Column<DateTime>(nameof(PSSEscortSettings.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSEscortSettings.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSEscortSettings).Name);

            string queryString = string.Format("INSERT INTO {0} ([{1}], [{2}], [{3}], [{4}]) VALUES (1, 1,'{5}', '{5}')", tableName, nameof(PSSEscortSettings.AdminToAssignOfficers) + "_Id", nameof(PSSEscortSettings.WorkFlowDefinition) + "_Id", nameof(PSSEscortSettings.CreatedAtUtc), nameof(PSSEscortSettings.UpdatedAtUtc), DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));

            SchemaBuilder.ExecuteSql(queryString);
            
            return 1;
        }

    }
}