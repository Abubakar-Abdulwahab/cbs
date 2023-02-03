using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class ReferenceDataBusinessPremisesMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ReferenceDataBusinessPremises).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<Int64>("ReferenceDataAsset_Id", column => column.NotNull())
                            .Column<int>("OrganizationType", column => column.NotNull())
                            .Column<int>("NoofEmployees", column => column.NotNull())
                            .Column<DateTime>("CommencementDate", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}