using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class SettlementEngineDetailsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(SettlementEngineDetails).Name, table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("JSONModel", column => column.NotNull().Unlimited())
                            .Column<DateTime>("TimeFired", column => column.NotNull())
                            .Column<string>("Params", column => column.NotNull())
                            .Column<decimal>("Amount", column => column.NotNull())
                            .Column<int>("TransactionCount", column => column.NotNull())
                            .Column<int>("SettlementId", column => column.NotNull())
                            .Column<bool>("Error", column => column.NotNull())
                            .Column<string>("SettlementEngineResponseJSON", column => column.NotNull().Unlimited())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                 );
            return 1;
        }

    }
}