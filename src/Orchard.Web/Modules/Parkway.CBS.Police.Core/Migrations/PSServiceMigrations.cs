using System;
using Orchard.Data.Migration;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.Migrations
{

    public class PSServiceMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(PSService).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name", column => column.NotNull().Unique())
                    .Column<bool>("IsActive", column => column.WithDefault(true).NotNull())
                    .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                    .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PSService).Name, table => table.AddColumn("ServiceType", System.Data.DbType.Int32, column => column.Nullable()));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PSService).Name, table => table.AddColumn("ServicePrefix", System.Data.DbType.String, column => column.Nullable()));

            string tableName = SchemaBuilder.TableDbName(typeof(PSService).Name);

            string queryString = string.Format("UPDATE {0} SET [ServicePrefix] = EXT WHERE ServiceType = {1}", tableName, (int)PSSServiceTypeDefinition.Extract);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("UPDATE {0} SET [ServicePrefix] = ESC WHERE ServiceType = {1}", tableName, (int)PSSServiceTypeDefinition.Escort);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN ServicePrefix nvarchar(5) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(PSService).Name, table => table.AddColumn("FlowDefinition_Id", System.Data.DbType.Int32, column => column.Nullable()));
            string tableName = SchemaBuilder.TableDbName(typeof(PSService).Name);

            string queryString = string.Format("UPDATE {0} SET [FlowDefinition_Id] = 1", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN FlowDefinition_Id int NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(PSService).Name, table => table.AddColumn("ServiceNotes", System.Data.DbType.String, column => column.Nullable().Unlimited()));
            string tableName = SchemaBuilder.TableDbName(typeof(PSService).Name);

            string initialNote = "'<li><div class=li-disc><div class=li-inner-disc></div></div><p>You will be required to pay a non-refundable application fee.</p></li><li><div class=li-disc><div class=li-inner-disc></div></div><p>The Nigerian Police Force reserves the right to approve or deny your request based on its guidelines or availability of resources.</p></li><li><div class=li-disc><div class=li-inner-disc></div> </div> <p>All Police Officers on Escort duty or Protection services are to be treated with utmost respect. Any complaint of mistreatment may attract strict penalties.</p></li>';";

            string queryString = string.Format("UPDATE {0} SET [ServiceNotes] = {1}", tableName, initialNote);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN ServiceNotes nvarchar(MAX) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 5;
        }


        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(PSService).Name, table => table.AddColumn(nameof(PSService.HasDifferentialWorkFlow), System.Data.DbType.Boolean, column => column.WithDefault(false).NotNull()));
            string tableName = SchemaBuilder.TableDbName(typeof(PSService).Name);

            string queryString = string.Format("UPDATE {0} SET [HasDifferentialWorkFlow] = 0", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN HasDifferentialWorkFlow bit NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 6;
        }


        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(PSService).Name, table => table.AddColumn(nameof(PSService.SideNotePartialName), System.Data.DbType.String));
            string tableName = SchemaBuilder.TableDbName(typeof(PSService).Name);

            string queryString = string.Format("UPDATE {0} SET [SideNotePartialName] = 'PartialName'", SchemaBuilder.TableDbName(typeof(PSService).Name));
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN SideNotePartialName nvarchar(50) NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 7;
        }

    }
}