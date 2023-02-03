using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEAPIRequestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PAYEAPIRequest).Name,
                table => table
                            .Column<Int64>(nameof(PAYEAPIRequest.Id), column => column.PrimaryKey().Identity())
                            .Column<Int64>(nameof(PAYEAPIRequest.TaxEntity)+"_Id", column => column.NotNull())
                            .Column<Int64>(nameof(PAYEAPIRequest.PAYEBatchRecordStaging)+"_Id", column => column.Nullable())
                            .Column<int>(nameof(PAYEAPIRequest.RequestedByExpertSystem)+"_Id", column => column.NotNull())
                            .Column<int>(nameof(PAYEAPIRequest.BatchLimit), column => column.NotNull())
                            .Column<int>(nameof(PAYEAPIRequest.ProcessingStage), column => column.NotNull())
                            .Column<bool>(nameof(PAYEAPIRequest.ProcessingCompleted), column => column.NotNull().WithDefault(false))
                            .Column<string>(nameof(PAYEAPIRequest.BatchIdentifier), column => column.NotNull())
                            .Column<string>(nameof(PAYEAPIRequest.CallbackURL), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEAPIRequest.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(PAYEAPIRequest.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PAYEAPIRequest).Name);
            string unqiueQuery = string.Format("ALTER TABLE [dbo].[{0}] ADD constraint PAYEAPIRequest_Unique_Constraint UNIQUE([{1}],[{2}]);", tableName, nameof(PAYEAPIRequest.RequestedByExpertSystem) + "_Id", nameof(PAYEAPIRequest.BatchIdentifier));
            SchemaBuilder.ExecuteSql(unqiueQuery);

            return 1;
        }

    }
}