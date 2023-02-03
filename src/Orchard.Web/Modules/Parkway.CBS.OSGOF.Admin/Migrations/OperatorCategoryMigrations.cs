using Orchard.Data.Migration;
using Parkway.CBS.OSGOF.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Migrations
{
    public class OperatorCategoryMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(OperatorCategory).Name,
                table => table
                            .Column<int>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("TaxProfileCategory_Id", column => column.NotNull().Unique())
                            .Column<string>("ShortName", column => column.NotNull().WithLength(1).Unique())
                            .Column<decimal>("Amount", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }
    }
}