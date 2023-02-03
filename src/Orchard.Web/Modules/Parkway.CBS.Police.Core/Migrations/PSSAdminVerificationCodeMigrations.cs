using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSAdminVerificationCodeMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSAdminVerificationCode).Name,
                table => table
                    .Column<Int64>(nameof(PSSAdminVerificationCode.Id), column => column.PrimaryKey().Identity())
                    .Column<int>($"{nameof(PSSAdminVerificationCode.AdminUser)}_Id", column => column.NotNull())
                    .Column<int>(nameof(PSSAdminVerificationCode.ResendCount), column => column.NotNull())
                    .Column<int>(nameof(PSSAdminVerificationCode.VerificationType), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSAdminVerificationCode.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSAdminVerificationCode.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
    }
}