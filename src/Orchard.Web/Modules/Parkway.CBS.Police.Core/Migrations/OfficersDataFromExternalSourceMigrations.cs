using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class OfficersDataFromExternalSourceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(OfficersDataFromExternalSource).Name,
                table => table
                    .Column<Int64>(nameof(OfficersDataFromExternalSource.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(OfficersDataFromExternalSource.Name), column => column.NotNull().WithLength(200))
                    .Column<string>(nameof(OfficersDataFromExternalSource.IPPISNumber), column => column.NotNull().WithLength(50))
                    .Column<string>(nameof(OfficersDataFromExternalSource.ServiceNunber), column => column.NotNull().WithLength(50))
                    .Column<string>(nameof(OfficersDataFromExternalSource.PhoneNumber), column => column.NotNull().WithLength(20))
                    .Column<string>(nameof(OfficersDataFromExternalSource.GenderCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.Gender), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.RankName), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.RankCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.StateName), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.StateCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.Command), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.CommandCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.LGAName), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.LGACode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.DateOfBirth), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.StateOfOrigin), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.AccountNumber), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.BankCode), column => column.NotNull())
                    .Column<string>(nameof(OfficersDataFromExternalSource.RequestIdentifier), column => column.NotNull())
                    .Column<int>(nameof(OfficersDataFromExternalSource.RequestItemSN), column => column.NotNull())
                    .Column<DateTime>(nameof(OfficersDataFromExternalSource.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(OfficersDataFromExternalSource.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

    }
}