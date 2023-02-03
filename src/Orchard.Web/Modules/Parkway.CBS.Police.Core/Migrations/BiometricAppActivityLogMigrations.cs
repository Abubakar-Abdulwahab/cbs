using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class BiometricAppActivityLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(BiometricAppActivityLog).Name,
                table => table
                    .Column<int>(nameof(BiometricAppActivityLog.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(BiometricAppActivityLog.MacAddress), column => column.NotNull())
                    .Column<string>(nameof(BiometricAppActivityLog.IPAddress), column => column.NotNull())
                    .Column<string>(nameof(BiometricAppActivityLog.Version), column => column.NotNull())
                    .Column<DateTime>(nameof(BiometricAppActivityLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(BiometricAppActivityLog.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}