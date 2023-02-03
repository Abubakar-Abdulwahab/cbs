using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class AddressMigration : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(Address).Name, table => table
                  .Column<int>("Id", c => c.Identity().PrimaryKey())
                  .Column<string>("HouseNumber")
                  .Column<string>("StreetName")
                  .Column<string>("District")
                  .Column<string>("City")
                  .Column<string>("State")
                  .Column<string>("Country")
                  .Column<string>("PMB")
                  .Column<string>("CO")
                  .Column("Town", System.Data.DbType.String)
                  .Column<string>("AdditionalInformation")
                  .Column<int>("Applicant_Id")
                  .Column<DateTime>("CreatedAtUtc", c => c.Nullable())
                  .Column<DateTime>("UpdatedAtUtc", c => c.Nullable())
                 );
            return 1;
        }

    }
}