using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PSSHangfireSettlementReferenceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSSHangfireSettlementReference).Name,
                table => table
                    .Column<Int64>(nameof(PSSHangfireSettlementReference.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(PSSHangfireSettlementReference.HangfireJobId), column => column.NotNull().Unique())
                    .Column<Int64>(nameof(PSSHangfireSettlementReference.ReferenceId), column => column.NotNull())
                    .Column<int>(nameof(PSSHangfireSettlementReference.ReferenceType), column => column.NotNull().WithLength(100))
                    .Column<DateTime>(nameof(PSSHangfireSettlementReference.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PSSHangfireSettlementReference.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PSSHangfireSettlementReference).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint PSSHANGFIRESETTLEMENTREFERENCE_UNIQUE_CONSTRAINT UNIQUE([{1}], [{2}]); ", tableName, nameof(PSSHangfireSettlementReference.ReferenceId), nameof(PSSHangfireSettlementReference.ReferenceType)));
            return 1;
        }
    }
}