using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSServiceRequestFlowApproverStagingMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSServiceRequestFlowApproverStaging).Name,
                table => table
                    .Column<int>(nameof(PSServiceRequestFlowApproverStaging.Id), column => column.PrimaryKey().Identity())
                    .Column<int>(nameof(PSServiceRequestFlowApproverStaging.FlowDefinitionLevel) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSServiceRequestFlowApproverStaging.AssignedApprover) + "_Id", column => column.NotNull())
                    .Column<int>(nameof(PSServiceRequestFlowApproverStaging.PSSAdminUser) + "_Id", column => column.NotNull())
                    .Column<bool>(nameof(PSServiceRequestFlowApproverStaging.IsDeleted), column => column.NotNull().WithDefault(false))
                    .Column<string>(nameof(PSServiceRequestFlowApproverStaging.Reference), column => column.NotNull())
                    .Column<DateTime>(nameof(PSServiceRequestFlowApproverStaging.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSServiceRequestFlowApproverStaging.UpdatedAtUtc), column => column.NotNull())
                );

            return 1;
        }
    }
}