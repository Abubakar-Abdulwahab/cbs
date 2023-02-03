using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class TINValidationResponseMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TINValidationResponse).Name,
                table => table
                            .Column<Int64>(nameof(TINValidationResponse.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(TINValidationResponse.TIN), c => c.Nullable().WithLength(20))
                            .Column<string>(nameof(TINValidationResponse.JTBTIN), c => c.Nullable().WithLength(20))
                            .Column<string>(nameof(TINValidationResponse.TaxPayerName), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(TINValidationResponse.Address), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(TINValidationResponse.TaxOfficerId), c => c.Nullable())
                            .Column<string>(nameof(TINValidationResponse.TaxOfficeName), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(TINValidationResponse.TaxPayerType), c => c.Nullable())
                            .Column<string>(nameof(TINValidationResponse.RCNumber), c => c.Nullable())
                            .Column<string>(nameof(TINValidationResponse.Email), c => c.Nullable())
                            .Column<string>(nameof(TINValidationResponse.Phone), c => c.Nullable().WithLength(15))
                            .Column<DateTime>(nameof(TINValidationResponse.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(TINValidationResponse.UpdatedAtUtc), column => column.NotNull())
                            .Column<string>(nameof(TINValidationResponse.ResponseDump), c => c.Nullable().Unlimited())
                );
            return 1;
        }
    }
}