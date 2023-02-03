using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceOfficerMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceOfficer).Name,
                table => table
                    .Column<int>(nameof(PoliceOfficer.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PoliceOfficer.Name), column => column.NotNull().WithLength(200))
                    .Column<Int64>(nameof(PoliceOfficer.Rank)+"_Id", column => column.NotNull())
                    .Column<string>(nameof(PoliceOfficer.IdentificationNumber), column => column.NotNull().WithLength(100).Unique())
                    .Column<string>(nameof(PoliceOfficer.IPPISNumber), column => column.NotNull().WithLength(100).Unique())
                    .Column<int>(nameof(PoliceOfficer.Command)+"_Id", column => column.NotNull())
                    .Column<bool>(nameof(PoliceOfficer.IsActive), column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceOfficer.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PoliceOfficer.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PoliceOfficer).Name, table => table.AddColumn(nameof(Gender) + "_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PoliceOfficer).Name, table => table.AddColumn(nameof(PoliceOfficer.AccountNumber), System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(PoliceOfficer).Name, table => table.AddColumn(nameof(PoliceOfficer.BankCode), System.Data.DbType.String, column => column.Nullable()));
            return 2;
        }

    }
}