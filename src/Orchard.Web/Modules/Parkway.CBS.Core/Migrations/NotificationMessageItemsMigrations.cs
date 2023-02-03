using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class NotificationMessageItemsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NotificationMessageItems).Name, table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("NotificationMessage_Id", column => column.NotNull())
                            .Column<string>("KeyName", column => column.Nullable())
                            .Column<string>("Value", column => column.NotNull().Unlimited())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                 );
            return 1;
        }

    }
}