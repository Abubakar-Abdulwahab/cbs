using System;
using Orchard.Data.Migration;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.Police.Core.Migrations
{
    public class ProposedEscortOfficerMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ProposedEscortOfficer).Name,
                table => table
                    .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                    .Column<Int64>("EscortDetails_Id", column => column.NotNull())
                    .Column<int>("Officer_Id", column => column.NotNull())
                    .Column<decimal>("EscortRankRate", column => column.NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(ProposedEscortOfficer).Name);
            SchemaBuilder.AlterTable(typeof(ProposedEscortOfficer).Name, table => table.AddColumn(nameof(ProposedEscortOfficer.OfficerLog)+"_Id", System.Data.DbType.Int64, column => column.Nullable()));
            string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN Officer_Id int NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(ProposedEscortOfficer).Name, table => table.AddColumn(nameof(ProposedEscortOfficer.IsDeleted), System.Data.DbType.Boolean, column => column.Nullable().WithDefault(false)));

            string tableName = SchemaBuilder.TableDbName(typeof(ProposedEscortOfficer).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = 0", tableName, nameof(ProposedEscortOfficer.IsDeleted));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} bit NOT NULL", tableName, nameof(ProposedEscortOfficer.IsDeleted));
            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }

    }
}