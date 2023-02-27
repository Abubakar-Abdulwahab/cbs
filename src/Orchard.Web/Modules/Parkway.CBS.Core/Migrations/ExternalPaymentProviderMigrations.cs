using System;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using Orchard.Data.Migration.Schema;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Migrations
{
    public class ExternalPaymentProviderMigrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(ExternalPaymentProvider).Name,
                table => table
                            .Column<int>("Id", column => column.PrimaryKey().Identity())
                            .Column<int>("Identifier", column => column.NotNull())
                            .Column<string>("Name", column => column.NotNull())
                            .Column<bool>("IsActive", column => column.NotNull().WithDefault(true))
                            .Column<int>("AddedBy_Id", column => column.NotNull())
                            .Column<string>("ClassImplementation", column => column.Nullable())
                            .Column<string>("ClientID", column => column.NotNull().Unique())
                            .Column<string>("ClientSecret", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(ExternalPaymentProvider).Name);

            string queryString = $"INSERT INTO {tableName} ([Identifier],[Name],[IsActive],[AddedBy_Id],[ClientID],[ClientSecret],[CreatedAtUtc],[UpdatedAtUtc]) VALUES ({(int)PaymentProvider.Bank3D}, {"'" + PaymentProvider.Bank3D.ToDescription() + "'"},{"'" + true + "'"},{2},{"'" + Utilities.Util.OnWayHashThis(PaymentProvider.Bank3D.ToDescription(), Utilities.AppSettingsConfigurations.EncryptionSecret) + "'"},{"'89450-8ujf-rerf-rek5-jfksbnt49ops'"},{"'" + DateTime.Now.ToString() + "'"},{"'" + DateTime.Now.ToString() + "'"});";

            queryString += $"INSERT INTO {tableName} ([Identifier],[Name],[IsActive],[AddedBy_Id],[ClientID],[ClientSecret],[CreatedAtUtc],[UpdatedAtUtc]) VALUES ({(int)PaymentProvider.Remita}, {"'" + PaymentProvider.Remita.ToDescription() + "'"},{"'" + true + "'"},{2},{"'00LfbhOacs7yM6xZxEho1w1Rfz4SvGCbnyDuJ7XHSvU='"},{"'00LfbhOacs7yM6xZxEho1w1Rfz4SvGCbnyDuJ7XHSvU='"},{"'" + DateTime.Now.ToString() + "'"},{"'" + DateTime.Now.ToString() + "'"});";

            queryString += $"INSERT INTO {tableName} ([Identifier],[Name],[IsActive],[AddedBy_Id],[ClientID],[ClientSecret],[CreatedAtUtc],[UpdatedAtUtc]) VALUES ({(int)PaymentProvider.PayDirect}, {"'" + PaymentProvider.PayDirect.ToDescription() + "'"},{"'" + true + "'"},{2},{"'" + Utilities.Util.OnWayHashThis(PaymentProvider.PayDirect.ToDescription(), Utilities.AppSettingsConfigurations.EncryptionSecret) + "'"},{"'" + Utilities.Util.StrongRandom() + "'"},{"'" + DateTime.Now.ToString() + "'"},{"'" + DateTime.Now.ToString() + "'"});";

            queryString += $"INSERT INTO {tableName} ([Identifier],[Name],[IsActive],[AddedBy_Id],[ClientID],[ClientSecret],[CreatedAtUtc],[UpdatedAtUtc]) VALUES ({(int)PaymentProvider.NIBSS}, {"'" + PaymentProvider.NIBSS.ToDescription() + "'"},{"'" + true + "'"},{2},{"'" + Utilities.Util.OnWayHashThis(PaymentProvider.NIBSS.ToDescription(), Utilities.AppSettingsConfigurations.EncryptionSecret) + "'"},{"'" + Utilities.Util.StrongRandom() + "'"},{"'" + DateTime.Now.ToString() + "'"},{"'" + DateTime.Now.ToString() + "'"});";


            queryString += $"INSERT INTO {tableName} ([Identifier],[Name],[IsActive],[AddedBy_Id],[ClientID],[ClientSecret],[CreatedAtUtc],[UpdatedAtUtc]) VALUES ({(int)PaymentProvider.Readycash}, {"'" + PaymentProvider.Readycash.ToDescription() + "'"},{"'" + true + "'"},{2},{"'" + Utilities.Util.OnWayHashThis(PaymentProvider.Readycash.ToDescription(), Utilities.AppSettingsConfigurations.EncryptionSecret) + "'"},{"'M+9uX1ZznM2bAUkbFn1Y6FZQ5BOmhSFbOqjUHRMZHg13zarszhYmsFIRNaSL'"},{"'" + DateTime.Now.ToString() + "'"},{"'" + DateTime.Now.ToString() + "'"});";

            SchemaBuilder.ExecuteSql(queryString);
            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(ExternalPaymentProvider).Name, table => table.DropColumn("Identifier"));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(ExternalPaymentProvider).Name, table => table.AddColumn(nameof(ExternalPaymentProvider.AllowAgentFeeAddition), System.Data.DbType.Boolean, col => col.WithDefault(false)));

            string tableName = SchemaBuilder.TableDbName(typeof(ExternalPaymentProvider).Name);

            string queryString = string.Format("UPDATE {0} SET [{1}] = {2}", tableName, nameof(ExternalPaymentProvider.AllowAgentFeeAddition), 0.00m);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} bit NOT NULL", tableName, nameof(ExternalPaymentProvider.AllowAgentFeeAddition));
            SchemaBuilder.ExecuteSql(queryString);

            return 3;
        }

    }
}