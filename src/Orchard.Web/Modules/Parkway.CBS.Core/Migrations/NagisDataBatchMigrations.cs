using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class NagisDataBatchMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NagisDataBatch).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("AdminUser_Id", column => column.NotNull())
                            .Column<int>("ProccessStage", column => column.NotNull())
                            .Column<int>("NumberOfRecords", column => column.Nullable())
                            .Column<string>("FilePath", column => column.Nullable().WithLength(1000))
                            .Column<string>("FileName", column => column.Nullable().WithLength(150))
                            .Column<string>("AdapterClassName", column => column.Nullable().WithLength(500))
                            .Column<bool>("ErrorOccurred", column => column.NotNull().WithDefault(false))
                            .Column<string>("ErrorMessage", column => column.Nullable().Unlimited())
                            .Column<int>("PercentageProgress", column => column.NotNull().WithDefault(0))
                            .Column<Int64>("NumberOfRecordSentToCashFlow", column => column.Nullable())
                            .Column<Int64>("GeneralBatchReference_Id", column => column.NotNull())
                            .Column<int>("StateModel_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(NagisDataBatch).Name);

            string queryString = string.Format("ALTER TABLE {0} add [BatchRef] as ((concat('NAGREF',case when len(CONVERT([varchar](10),[Id]))<(3) then '_000' else '_' end,[Id]))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}