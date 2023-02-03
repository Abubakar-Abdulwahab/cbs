using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSBiometricAppUserDetailMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSBiometricAppUserDetail).Name,
                table => table
                    .Column<int>(nameof(PSSBiometricAppUserDetail.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSSBiometricAppUserDetail.MacAddress), column => column.NotNull().WithLength(250).Unique())
                    .Column<string>(nameof(PSSBiometricAppUserDetail.Version), column => column.NotNull().WithLength(20))
                    .Column<int>(nameof(PSSBiometricAppUserDetail.Command) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PSSBiometricAppUserDetail.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSBiometricAppUserDetail.UpdatedAtUtc), column => column.NotNull()));

            return 1;
        }
    }
}