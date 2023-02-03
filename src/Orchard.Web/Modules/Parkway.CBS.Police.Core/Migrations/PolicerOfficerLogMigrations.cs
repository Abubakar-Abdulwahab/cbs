using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PolicerOfficerLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PolicerOfficerLog).Name,
                table => table
                    .Column<Int64>(nameof(PolicerOfficerLog.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PolicerOfficerLog.Name), column => column.NotNull().WithLength(200))
                    .Column<string>(nameof(PolicerOfficerLog.PhoneNumber), column => column.NotNull().WithLength(11))
                    .Column<Int64>(nameof(PolicerOfficerLog.Rank)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(PolicerOfficerLog.IdentificationNumber), column => column.NotNull())
                    .Column<string>(nameof(PolicerOfficerLog.IPPISNumber), column => column.NotNull())
                    .Column<int>(nameof(PolicerOfficerLog.Command) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(PolicerOfficerLog.Gender), column => column.NotNull().WithLength(10))
                    .Column<string>(nameof(PolicerOfficerLog.AccountNumber), column => column.NotNull())
                    .Column<string>(nameof(PolicerOfficerLog.BankCode), column => column.NotNull())
                    .Column<DateTime>(nameof(PolicerOfficerLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PolicerOfficerLog.UpdatedAtUtc), column => column.NotNull())
                )
                .AlterTable(typeof(PolicerOfficerLog).Name, table => table.CreateIndex("NameIndex", new string[] { nameof(PolicerOfficerLog.Name) }));

            return 1;
        }
    }
}