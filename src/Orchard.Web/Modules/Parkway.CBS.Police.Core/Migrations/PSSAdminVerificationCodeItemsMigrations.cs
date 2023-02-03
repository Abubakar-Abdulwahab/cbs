using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSAdminVerificationCodeItemsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSAdminVerificationCodeItems).Name,
                table => table
                    .Column<Int64>(nameof(PSSAdminVerificationCodeItems.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>($"{nameof(PSSAdminVerificationCodeItems.VerificationCode)}_Id", column => column.NotNull())
                    .Column<string>(nameof(PSSAdminVerificationCodeItems.CodeHash), column => column.NotNull().WithLength(250))
                    .Column<int>(nameof(PSSAdminVerificationCodeItems.State), column => column.NotNull().WithDefault(0))
                    .Column<DateTime>(nameof(PSSAdminVerificationCodeItems.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSAdminVerificationCodeItems.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }
    }
}