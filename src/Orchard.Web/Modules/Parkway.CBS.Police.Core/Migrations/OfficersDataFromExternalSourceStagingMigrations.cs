using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class OfficersDataFromExternalSourceStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(OfficersDataFromExternalSourceStaging).Name,
                table => table
                    .Column<Int64>(nameof(OfficersDataFromExternalSourceStaging.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.Name), column => column.NotNull().WithLength(200))
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.IPPISNumber), column => column.NotNull().WithLength(50))
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.ServiceNunber), column => column.NotNull().WithLength(50))
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.PhoneNumber), column => column.NotNull().WithLength(20))
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.GenderCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.Gender), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.RankName), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.RankCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.StateName), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.StateCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.Command), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.CommandCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.LGAName), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.LGACode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.DateOfBirth), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.StateOfOrigin), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.AccountNumber), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.BankCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.RequestIdentifier), column => column.NotNull())
                    .Column<int>(nameof(OfficersDataFromExternalSourceStaging.RequestItemSN), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSourceStaging.ExternalDataReference), column => column.NotNull())
                    .Column<DateTime>(nameof(OfficersDataFromExternalSourceStaging.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(OfficersDataFromExternalSourceStaging.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

    }
}