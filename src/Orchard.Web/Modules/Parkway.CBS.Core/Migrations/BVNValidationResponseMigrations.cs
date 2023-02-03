using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class BVNValidationResponseMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(BVNValidationResponse).Name,
                table => table
                            .Column<Int64>(nameof(BVNValidationResponse.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(BVNValidationResponse.PhoneNumber), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.EmailAddress), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.Gender), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.PhoneNumber2), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.LevelOfAccount), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.LgaOfOrigin), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.LgaOfResidence), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.MaritalStatus), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.NIN), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.NameOnCard), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.Nationality), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.ResidentialAddress), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.StateOfOrigin), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.StateOfResidence), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.Title), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.Base64Image), c => c.Nullable().Unlimited())
                            .Column<string>(nameof(BVNValidationResponse.ResponseCode), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.BVN), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.FirstName), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.MiddleName), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.LastName), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.DateOfBirth), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.RegistrationDate), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.EnrollmentBank), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.EnrollmentBranch), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.WatchListed), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.RequestId), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.ResponseDescription), c => c.Nullable().WithLength(500))
                            .Column<string>(nameof(BVNValidationResponse.UserId), c => c.Nullable().WithLength(500))
                            .Column<DateTime>(nameof(BVNValidationResponse.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(BVNValidationResponse.UpdatedAtUtc), column => column.NotNull())
                            .Column<string>(nameof(BVNValidationResponse.ResponseDump), c => c.Nullable().Unlimited())
                );
            return 1;
        }
    }
}