using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;


namespace Parkway.CBS.Police.Core.Migrations
{
    public class EscortViewRubricMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(EscortViewRubric).Name,
                table => table
                            .Column<int>(nameof(EscortViewRubric.Id), column => column.Identity().PrimaryKey())
                            .Column<int>(nameof(EscortViewRubric.RequestLevel) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(EscortViewRubric.ChildLevel) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(EscortViewRubric.PermissionType), column => column.NotNull())
                            .Column<bool>(nameof(EscortViewRubric.IsDeleted), column => column.NotNull().WithDefault(false))
                            .Column<DateTime>(nameof(EscortViewRubric.CreatedAtUtc), column => column.NotNull())
                            .Column<DateTime>(nameof(EscortViewRubric.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(EscortViewRubric).Name);
            string queryString = string.Format("ALTER TABLE[dbo].[{0}] ADD constraint ESCORT_VIEW_RUBRIC_UNIQUE_CONSTRAINT UNIQUE([{1}], [{2}], [{3}])", tableName, nameof(EscortViewRubric.RequestLevel) + "_Id", nameof(EscortViewRubric.ChildLevel) + "_Id", nameof(EscortViewRubric.PermissionType));
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }
    }
}