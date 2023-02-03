using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ExtractDetailsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ExtractDetails).Name,
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
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn("SelectedCategory", System.Data.DbType.Int32, column => column.WithDefault(0)));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn("SelectedSubCategory", System.Data.DbType.Int32, column => column.WithDefault(0)));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.IsIncidentReported), System.Data.DbType.Boolean, column => column.NotNull().WithDefault(false)));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.AffidavitNumber), System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.IncidentReportedDate), System.Data.DbType.String, column => column.Nullable()));
            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn("Details", System.Data.DbType.String, column => column.Nullable().WithLength(1000)));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn("Action", System.Data.DbType.String, column => column.Nullable().WithLength(1000)));
            return 4;
        }


        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.DropColumn("Details"));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.DropColumn("Action"));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.DiarySerialNumber), System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.IncidentDateAndTime), System.Data.DbType.DateTime, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.CrossReferencing), System.Data.DbType.String, column => column.Nullable().WithLength(10)));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.Content), System.Data.DbType.String, column => column.Nullable().WithLength(1000)));
            return 5;
        }


        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.DPOServiceNumber), System.Data.DbType.String, column => column.WithLength(50).Nullable()));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.DPOName), System.Data.DbType.String, column => column.WithLength(100).Nullable()));
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.DPOAddedBy)+"_Id", System.Data.DbType.Int32, column => column.Nullable()));
            return 6;
        }


        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.DPORankCode), System.Data.DbType.String, column => column.Nullable().WithLength(20)));
            return 7;
        }


        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(ExtractDetails).Name, table => table.AddColumn(nameof(ExtractDetails.AffidavitDateOfIssuance), System.Data.DbType.DateTime, column => column.Nullable()));
            return 8;
        }
    }
}