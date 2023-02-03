using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Migrations
{
    public class AccessRoleUserMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(AccessRoleUser).Name,
                table => table
                            .Column<Int64>("Id", column => column.Identity().PrimaryKey())
                            .Column<int>("AccessRole_Id", column => column.NotNull())
                            .Column<int>("User_Id", column => column.NotNull())
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(AccessRoleUser).Name);

            string queryString = string.Format("ALTER TABLE {0} add [CompositeUnique] as ((concat('A-',CONVERT([nvarchar](50),[AccessRole_Id]),'-U-',CONVERT([nvarchar](50),[User_Id])))) PERSISTED", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 1;
        }
    }
}