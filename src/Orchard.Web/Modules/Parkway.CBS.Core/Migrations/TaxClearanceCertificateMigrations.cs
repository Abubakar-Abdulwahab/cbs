using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxClearanceCertificateMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxClearanceCertificate).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("ApplicantName", column => column.NotNull())
                            .Column<string>("ResidentialAddress", column => column.NotNull())
                            .Column<string>("OfficeAddress", column => column.Nullable())
                            .Column<string>("TCCNumber", column => column.Nullable().WithLength(50))
                            .Column<Int64>("TaxEntity_Id", column => column.Nullable())
                            .Column<int>("ApplicationYear", column => column.NotNull())
                            .Column<Int64>("TaxClearanceCertificateRequest_Id", column => column.NotNull())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<string>("TotalIncomeAndTaxAmountPaidWithYear", column => column.NotNull())
                            .Column<int>("RevenueOfficerSignature_Id", column => column.NotNull())
                            .Column<int>("DirectorOfRevenueSignature_Id", column => column.NotNull())
                            .Column("RevenueOfficerSignatureBlob", System.Data.DbType.String, column => column.NotNull().Unlimited())
                            .Column<string>("RevenueOfficerSignatureContentType",column => column.NotNull().WithLength(10))
                            .Column("DirectorOfRevenueSignatureBlob", System.Data.DbType.String, column => column.NotNull().Unlimited())
                            .Column<string>("DirectorOfRevenueSignatureContentType", column => column.NotNull().WithLength(10))
                            .Column<string>("TaxClearanceCertificateTemplate", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }
    }
}