using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class NINValidationResponseMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NINValidationResponse).Name,
                table => table
                            .Column<Int64>(nameof(NINValidationResponse.Id), c => c.Identity().PrimaryKey())
                            .Column<string>(nameof(NINValidationResponse.BatchId), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.BirthCountry), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.BirthDate), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.BirthLga), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.BirthState), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.CardStatus), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.CentralID), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.DocumentNo), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.EducationalLevel), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.Email), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.EmploymentStatus), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.FirstName), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.Gender), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.Height), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.MaritalStatus), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.MiddleName), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NativeSpokenLang), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NextOfKinAddress1), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NextOfKinAddress2), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NextOfKinFirstName), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NextOfKinLGA), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NextOfKinMiddleName), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NextOfKinState), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NextOfKinSurname), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NextOfKinTown), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.NIN), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.Photo), c => c.Nullable().Unlimited())
                            .Column<string>(nameof(NINValidationResponse.Profession), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.Religion), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.ResidenceAdressLine1), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.ResidenceLGA), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.ResidenceState), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.ResidenceStatus), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.ResidenceTown), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.SelfOriginLGA), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.SelfOriginPlace), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.SelfOriginState), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.Signature), c => c.Nullable().Unlimited())
                            .Column<string>(nameof(NINValidationResponse.Surname), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.TelephoneNo), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.Title), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(NINValidationResponse.TrackingId), c => c.Nullable().WithLength(500))
                            .Column<DateTime>(nameof(NINValidationResponse.CreatedAtUtc), c => c.NotNull())
                            .Column<DateTime>(nameof(NINValidationResponse.UpdatedAtUtc), c => c.NotNull())
                            .Column<string>(nameof(NINValidationResponse.ResponseDump), c => c.Nullable().Unlimited())
                );
            return 1;
        }
    }
}