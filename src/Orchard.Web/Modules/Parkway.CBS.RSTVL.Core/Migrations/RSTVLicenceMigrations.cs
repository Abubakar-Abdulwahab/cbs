using Orchard.Data.Migration;
using Parkway.CBS.RSTVL.Core.Models;
using Parkway.CBS.RSTVL.Core.Models.Enums;
using System;

namespace Parkway.CBS.RSTVL.Core.Migrations
{
    public class RSTVLicenceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RSTVLicence).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("CustomerReference", column => column.Nullable())
                    .Column<int>("State_Id", column => column.NotNull())
                    .Column<int>("LGA_Id", column => column.NotNull())
                    .Column<int>("MDA_Id", column => column.NotNull())
                    .Column<int>("RevenueHead_Id", column => column.NotNull())
                    .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                    .Column<Int64>("Invoice_Id", column => column.NotNull())
                    .Column<int>("Year", column => column.NotNull())
                    .Column<int>("ClaimedBy_Id", column => column.Nullable())
                    .Column<int>("Status", column => column.NotNull().WithDefault((int)RSTVLicenceStatus.Unclaimed))
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}