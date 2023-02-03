using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class InvestigationReportDetailsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(InvestigationReportDetails).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("Request_Id", column => column.NotNull())
                    .Column<string>("RequestReason", column => column.NotNull().WithLength(1000))
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(InvestigationReportDetails).Name, table => table.AddColumn("SelectedCategory", System.Data.DbType.Int32, column => column.WithDefault(0)));
            SchemaBuilder.AlterTable(typeof(InvestigationReportDetails).Name, table => table.AddColumn("SelectedSubCategory", System.Data.DbType.Int32, column => column.WithDefault(0)));
            return 2;
        }

    }
}