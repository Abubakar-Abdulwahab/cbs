using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceOfficerCommandRankLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceOfficerCommandRankLog).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("PoliceOfficer_Id", column => column.NotNull())
                    .Column<Int64>("Rank_Id", column => column.NotNull())
                    .Column<int>("Command_Id", column => column.NotNull())
                    .Column<int>("AddedBy_Id", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}