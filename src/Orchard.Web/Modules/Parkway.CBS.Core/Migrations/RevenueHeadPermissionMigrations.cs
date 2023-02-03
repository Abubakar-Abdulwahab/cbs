using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.Migrations
{
    public class RevenueHeadPermissionMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(RevenueHeadPermission).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("Name", column => column.NotNull().Unique().WithLength(100))
                            .Column<string>("Description", column => column.NotNull().WithLength(500))
                            .Column<bool>("IsActive", column => column.WithDefault(true))
                            .Column<Int64>("LastUpdatedBy_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(RevenueHeadPermission).Name);
            string nameCol = nameof(RevenueHeadPermission.Name);
            string descriptionCol = nameof(RevenueHeadPermission.Description);
            string isActiveCol = nameof(RevenueHeadPermission.IsActive);
            string lastUpdatedByCol = nameof(RevenueHeadPermission.LastUpdatedBy) + "_Id";
            string createdAtCol = nameof(RevenueHeadPermission.CreatedAtUtc);
            string updatedAtCol = nameof(RevenueHeadPermission.UpdatedAtUtc);

            string queryString = $"INSERT INTO {tableName} ([{nameCol}],[{descriptionCol}],[{isActiveCol}],[{lastUpdatedByCol}],[{createdAtCol}],[{updatedAtCol}]) VALUES ({"'" + EnumExpertSystemPermissions.CanMakePayePayments + "'"},{"'" + EnumExpertSystemPermissions.CanMakePayePayments.ToDescription() + "'"},{"'" + true + "'"},{2},{"'" + DateTime.Now.ToString() + "'"},{"'" + DateTime.Now.ToString() + "'"});";

            queryString += $"INSERT INTO {tableName} ([{nameCol}],[{descriptionCol}],[{isActiveCol}],[{lastUpdatedByCol}],[{createdAtCol}],[{updatedAtCol}]) VALUES ({"'" + EnumExpertSystemPermissions.GenerateInvoice + "'"},{"'" + EnumExpertSystemPermissions.GenerateInvoice.ToDescription() + "'"},{"'" + true + "'"},{2},{"'" + DateTime.Now.ToString() + "'"},{"'" + DateTime.Now.ToString() + "'"});";

            SchemaBuilder.ExecuteSql(queryString);
            return 2;
        }

    }
}