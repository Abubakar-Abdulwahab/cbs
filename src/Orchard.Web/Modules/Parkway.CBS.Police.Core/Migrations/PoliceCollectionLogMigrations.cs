using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class PoliceCollectionLogMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PoliceCollectionLog).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("TransactionLog_Id", column => column.NotNull())
                    .Column<Int64>("Request_Id", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PoliceCollectionLog).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint TransactionLog_UQ_Constraint UNIQUE([{1}]); ", tableName, nameof(PoliceCollectionLog.TransactionLog) + "_Id"));

            return 2;
        }

    }
}