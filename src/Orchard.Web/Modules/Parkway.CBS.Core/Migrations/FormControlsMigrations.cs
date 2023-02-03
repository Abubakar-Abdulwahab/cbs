using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class FormControlsMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(FormControl).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>("Name", column => column.NotNull().Unique())
                            .Column<string>("TechnicalName", column => column.NotNull().Unique())
                            .Column<int>("ControlTypeNumber", column => column.NotNull())
                            .Column<int>("ControlTypeDropDownNumber", column => column.Nullable())
                            .Column<int>("ElementType", column => column.NotNull())
                            .Column<bool>("DefaultStatus")
                            .Column<string>("FriendlyName", column => column.NotNull())
                            .Column<string>("LabelText", column => column.WithLength(255))
                            .Column<string>("HintText", column => column.WithLength(255).Nullable())
                            .Column<string>("PlaceHolderText", column => column.WithLength(255).Nullable())
                            .Column<string>("Validators", column => column.Nullable().WithLength(255))
                            .Column<int>("AddedBy_Id")
                            .Column<int>("LastUpdatedBy_Id")
                            .Column<DateTime>("CreatedAtUtc")
                            .Column<DateTime>("UpdatedAtUtc")
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(FormControl).Name, table => table.AddColumn("PartialName", System.Data.DbType.String));
            SchemaBuilder.AlterTable(typeof(FormControl).Name, table => table.AddColumn("IsComplexValidator", System.Data.DbType.Boolean));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(FormControl).Name, table => table.AddColumn("PartialModelProvider", System.Data.DbType.String));
            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(FormControl).Name, table => table.AddColumn("ValidationProps", System.Data.DbType.String));
            return 4;
        }
    }
}