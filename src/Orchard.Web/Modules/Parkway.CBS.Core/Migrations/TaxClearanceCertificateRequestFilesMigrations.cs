using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxClearanceCertificateRequestFilesMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxClearanceCertificateRequestFiles).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("TaxClearanceCertificateRequest_Id", column => column.NotNull())
                            .Column<int>("TCCFileUploadTypeId", column => column.NotNull())
                            .Column<string>("OriginalFileName", column => column.NotNull())
                            .Column<string>("FilePath", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(TaxClearanceCertificateRequestFiles).Name, table => table.AddColumn("ContentType", System.Data.DbType.String, column => column.WithLength(100)));
            return 2;
        }

    }
}