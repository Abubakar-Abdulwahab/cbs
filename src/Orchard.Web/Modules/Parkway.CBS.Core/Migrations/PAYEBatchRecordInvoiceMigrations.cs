using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBatchRecordInvoiceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEBatchRecordInvoice).Name,
                table => table
                            .Column<Int64>(nameof(PAYEBatchRecordInvoice.Id), column => column.PrimaryKey().Identity())
                            .Column<Int64>(nameof(PAYEBatchRecordInvoice.PAYEBatchRecord) + "_Id", column => column.NotNull())
                            .Column<Int64>(nameof(PAYEBatchRecordInvoice.Invoice) + "_Id", column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBatchRecordInvoice.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEBatchRecordInvoice.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PAYEBatchRecordInvoice).Name);
            string unqiueQuery = string.Format("ALTER TABLE [dbo].[{0}] ADD constraint PAYEInvoice_Unique_Constraint UNIQUE([{1}],[{2}]);", tableName, nameof(PAYEBatchRecordInvoice.PAYEBatchRecord) + "_Id", nameof(PAYEBatchRecordInvoice.Invoice) + "_Id");
            SchemaBuilder.ExecuteSql(unqiueQuery);
            return 1;
        }
    }
}