using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSRequestApprovalDocumentPreviewLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSRequestApprovalDocumentPreviewLog).Name,
                table => table
                            .Column<Int64>("Id", column => column.Identity().PrimaryKey())
                            .Column<Int64>(nameof(PSSRequestApprovalDocumentPreviewLog.Request)+"_Id", column => column.NotNull())
                            .Column<int>(nameof(PSSRequestApprovalDocumentPreviewLog.Approver)+"_Id", column => column.NotNull())
                            .Column<int>(nameof(PSSRequestApprovalDocumentPreviewLog.FlowDefinitionLevel)+"_Id", column => column.NotNull())
                            .Column<string>(nameof(PSSRequestApprovalDocumentPreviewLog.RequestDocumentDraftBlob), column => column.NotNull().Unlimited())
                            .Column<bool>(nameof(PSSRequestApprovalDocumentPreviewLog.Confirmed), column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}