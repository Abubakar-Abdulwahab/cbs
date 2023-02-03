using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class HangfireJobReferenceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(HangfireJobReference).Name,
                table => table
                    .Column<Int64>(nameof(HangfireJobReference.Id), column => column.PrimaryKey().Identity())
                    .Column<string>(nameof(HangfireJobReference.HangfireJobId), column => column.NotNull())
                    .Column<string>(nameof(HangfireJobReference.JobReferenceNumber), column => column.NotNull())
                    .Column<DateTime>(nameof(HangfireJobReference.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(HangfireJobReference.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(HangfireJobReference).Name);
            string queryString = string.Format("CREATE NONCLUSTERED INDEX HangfireJobReference_JobReferenceNumber_INDEX ON dbo.{0}(JobReferenceNumber)", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 1;
        }
    }
}