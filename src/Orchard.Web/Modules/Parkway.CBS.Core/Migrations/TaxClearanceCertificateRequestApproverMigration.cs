using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxClearanceCertificateRequestApproverMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxClearanceCertificateRequestApprover).Name,
                table => table
                    .Column<int>(nameof(TaxClearanceCertificateRequestApprover.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(TaxClearanceCertificateRequestApprover.ApprovalLevelId), column => column.NotNull())
                    .Column<int>(nameof(TaxClearanceCertificateRequestApprover.AssignedApprover) + "_Id", column => column.NotNull())
                    .Column<string>(nameof(TaxClearanceCertificateRequestApprover.ContactEmail), column => column.NotNull())
                    .Column<string>(nameof(TaxClearanceCertificateRequestApprover.ContactPhoneNumber), column => column.NotNull())
                    .Column<DateTime>(nameof(TaxClearanceCertificateRequestApprover.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(TaxClearanceCertificateRequestApprover.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}