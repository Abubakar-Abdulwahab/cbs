using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class BankMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(Bank).Name,
                table => table
                            .Column<int>(nameof(Bank.Id), column => column.PrimaryKey().Identity())
                            .Column<string>(nameof(Bank.Name), column => column.NotNull().WithLength(200).Unique())
                            .Column<string>(nameof(Bank.Code), column => column.NotNull().WithLength(10).Unique())
                            .Column<bool>(nameof(Bank.IsDeleted), column => column.NotNull().WithDefault(false))
                            .Column<string>(nameof(Bank.ShortName), column => column.WithLength(50).NotNull().Unique())
                            .Column<DateTime>(nameof(Bank.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(Bank.UpdatedAtUtc), column => column.NotNull())
                );
            return 1;
        }

    }
}