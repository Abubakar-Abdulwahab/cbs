using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxClearanceCertificateRequestApprovalLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxClearanceCertificateRequestApprovalLog).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(TaxClearanceCertificateRequestApprovalLog.Request) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(TaxClearanceCertificateRequestApprovalLog.Status), column => column.NotNull())
                    .Column<int>(nameof(TaxClearanceCertificateRequestApprovalLog.ApprovalLevelId), column => column.NotNull())
                    .Column<int>(nameof(TaxClearanceCertificateRequestApprovalLog.AddedByAdminUser) +"_Id", column => column.NotNull())
                    .Column<string>(nameof(TaxClearanceCertificateRequestApprovalLog.Comment), column => column.NotNull().WithLength(500))
                    .Column<DateTime>(nameof(TaxClearanceCertificateRequestApprovalLog.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(TaxClearanceCertificateRequestApprovalLog.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }

    }
}