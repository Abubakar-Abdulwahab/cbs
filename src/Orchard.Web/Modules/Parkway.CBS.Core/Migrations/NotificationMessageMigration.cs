using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class NotificationMessageMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(NotificationMessage).Name, table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.NotNull())
                            .Column<Int64>("TaxPayer_Id", column => column.NotNull())
                            .Column<int>("NotificationTypeId", column => column.NotNull())
                            .Column<string>("Recipient", column => column.NotNull())
                            .Column<string>("MailSubject", column => column.Nullable())
                            .Column<string>("Body", column => column.NotNull().Unlimited())
                            .Column<int>("DeliveryStatusId", column => column.NotNull())
                            .Column<DateTime>("SentDate", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                 );
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(NotificationMessage).Name, table => table.DropColumn("MDA_Id"));
            SchemaBuilder.AlterTable(typeof(NotificationMessage).Name, table => table.DropColumn("RevenueHead_Id"));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(NotificationMessage).Name, table => table.DropColumn("Body"));
            SchemaBuilder.AlterTable(typeof(NotificationMessage).Name, table => table.DropColumn("MailSubject"));
            return 3;
        }
    }
}