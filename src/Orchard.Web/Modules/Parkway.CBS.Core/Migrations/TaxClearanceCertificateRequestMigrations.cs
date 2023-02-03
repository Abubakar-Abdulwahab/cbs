using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxClearanceCertificateRequestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxClearanceCertificateRequest).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("ApplicantName", column => column.NotNull())
                            .Column<string>("ResidentialAddress", column => column.NotNull())
                            .Column<string>("OfficeAddress", column => column.Nullable())
                            .Column<string>("Occupation", column => column.Nullable())
                            .Column<string>("PhoneNumber", column => column.NotNull())
                            .Column<string>("TIN", column => column.Nullable())
                            .Column<bool>("IsRentedApartment", column => column.WithDefault(false))
                            .Column<string>("LandlordName", column => column.Nullable())
                            .Column<string>("LandlordAddress", column => column.Nullable())
                            .Column<string>("RequestReason", column => column.NotNull())
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<bool>("HasPaidDevelopmentLevy", column => column.Nullable())
                            .Column<bool>("IsExempted", column => column.Nullable())
                            .Column<int>("ExemptionCategory", column => column.Nullable())
                            .Column<string>("FileName", column => column.Nullable())
                            .Column<string>("FilePath", column => column.Nullable())
                            .Column<int>("Status", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(TaxClearanceCertificateRequest).Name);

            string queryString = string.Format("ALTER TABLE {0} add [ApplicationNumber] as (rtrim('TCC_')+case when len(rtrim(CONVERT([nvarchar](20),[Id],0)))>(9) then CONVERT([nvarchar](20),[Id],0) else right(replicate((0),(10))+rtrim(CONVERT([nvarchar](10),[Id],0)),(10)) end) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("HusbandName", System.Data.DbType.String, column => column.WithLength(200)));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("HusbandAddress", System.Data.DbType.String, column => column.WithLength(500)));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("InstitutionName", System.Data.DbType.String, column => column.WithLength(500)));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("IdentificationNumber", System.Data.DbType.String, column => column.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("DevelopmentLevyInvoiceNumber", System.Data.DbType.String, column => column.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("ApprovedBy_Id", System.Data.DbType.Int32, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("Comment", System.Data.DbType.String, column => column.Nullable()));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.DropColumn("FileName"));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.DropColumn("FilePath"));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.DropColumn("HasPaidDevelopmentLevy"));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.DropColumn("IsExempted"));
            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("TCCNumber", System.Data.DbType.String, column => column.WithLength(50)));
            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.DropColumn("DevelopmentLevyInvoiceNumber"));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("DevelopmentLevyInvoice_Id", System.Data.DbType.Int64));
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn("ApplicationYear", System.Data.DbType.Int32));
            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.AddColumn(nameof(TaxClearanceCertificateRequest.ApprovalStatusLevelId), System.Data.DbType.Int32));
            return 7;
        }

        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.DropColumn("ApprovedBy_Id"));
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequest).Name, table => table.DropColumn("Comment"));
            return 8;
        }

    }
}