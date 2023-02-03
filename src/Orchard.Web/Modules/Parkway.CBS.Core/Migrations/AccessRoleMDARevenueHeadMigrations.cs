using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class AccessRoleMDARevenueHeadMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AccessRoleMDARevenueHead).Name,
                table => table
                            .Column<Int64>("Id", column => column.Identity().PrimaryKey())
                            .Column<int>("AccessRole_Id", column => column.NotNull())
                            .Column<int>("MDA_Id", column => column.NotNull())
                            .Column<int>("RevenueHead_Id", column => column.Nullable())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(AccessRoleMDARevenueHead).Name);

            string queryString = string.Format("ALTER TABLE {0} add [CompositeUnique] as ((concat('A-',CONVERT([nvarchar](50),[AccessRole_Id]),'-M-',CONVERT([nvarchar](50),[MDA_Id]),'-R-',case when [RevenueHead_Id] IS NOT NULL then CONVERT([nvarchar](50),[RevenueHead_Id]) else '0' end))) PERSISTED NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            string unqiueQuery = string.Format("ALTER TABLE[dbo].[{0}] ADD constraint Composite_Unique_Constraint UNIQUE([CompositeUnique]); ", tableName);
            SchemaBuilder.ExecuteSql(unqiueQuery);
            return 1;
        }
    }
}