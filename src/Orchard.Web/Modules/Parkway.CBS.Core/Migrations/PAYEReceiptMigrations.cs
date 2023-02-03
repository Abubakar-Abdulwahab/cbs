using Orchard;
using Orchard.Data.Migration;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using System;
using System.Linq;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEReceiptMigrations : DataMigrationImpl
    {

        private readonly IOrchardServices _orchardServices;

        public PAYEReceiptMigrations(IOrchardServices orchardServices) { _orchardServices = orchardServices; }

        public int Create()
        {
            Node setting = null;

            try
            {
                setting = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                       .Node.Where(k => k.Key == TenantConfigKeys.ReceiptPrefix.ToString()).FirstOrDefault();
            }
            catch (Exception)
            {
                setting = new Node { Value = string.Format("{0}", new Random().Next()) };
            }

            SchemaBuilder.CreateTable(typeof(PAYEReceipt).Name,
                table => table
                    .Column<Int64>(nameof(PAYEReceipt.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>("PAYEBatchItem_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PAYEReceipt.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PAYEReceipt.UpdatedAtUtc), column => column.NotNull())
                );

            //string tableName = SchemaBuilder.TableDbName(typeof(PAYEReceipt).Name);
            //string queryString = string.Format("ALTER TABLE {0} ADD [ReceiptNumber] as (concat('{1}','-')+case when len(rtrim(CONVERT([nvarchar](20),[Id],(0))))>(7) then CONVERT([nvarchar](20),[Id],(0)) else right(replicate((0),(7))+rtrim(CONVERT([nvarchar](7),[Id],(0))),(7)) end) PERSISTED;", tableName, setting.Value);

            //queryString += string.Format("CREATE NONCLUSTERED INDEX PAYE_RECEIPT_NUMBER_INDEX ON dbo.{0}(ReceiptNumber)", tableName);
            //SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(PAYEReceipt).Name, table => table.DropColumn("PAYEBatchItem_Id"));
            //SchemaBuilder.AlterTable(typeof(PAYEReceipt).Name, table => table.AddColumn("TransactionLog_Id", System.Data.DbType.Int64, column => column.Unique().NotNull()));
            SchemaBuilder.AlterTable(typeof(PAYEReceipt).Name, table => table.AddColumn("UtilizedAmount", System.Data.DbType.Decimal));
            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(PAYEReceipt).Name, table => table.AddColumn("UtilizationStatusId", System.Data.DbType.Int32));
            return 3;
        }

        public int UpdateFrom3()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PAYEReceipt).Name);
            //string indexDropQueryString = string.Format("IF EXISTS (SELECT name FROM sysindexes WHERE name = 'PAYE_RECEIPT_NUMBER_INDEX') DROP INDEX [dbo].[{0}].PAYE_RECEIPT_NUMBER_INDEX", tableName);
            //SchemaBuilder.ExecuteSql(indexDropQueryString);

            //SchemaBuilder.AlterTable(typeof(PAYEReceipt).Name, table => table.DropColumn("ReceiptNumber"));
            SchemaBuilder.AlterTable(typeof(PAYEReceipt).Name, table => table.AddColumn(nameof(PAYEReceipt.Receipt) + "_Id", System.Data.DbType.Int64, column => column.NotNull()));
            return 4;
        }


        public int UpdateFrom4()
        {
            //string tableName = SchemaBuilder.TableDbName(typeof(PAYEReceipt).Name);
            //string unqiueQuery = string.Format("ALTER TABLE[dbo].[{0}] ADD CONSTRAINT Receipt_TranLog_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(PAYEReceipt.Receipt)+"_Id", "TransactionLog_Id");

            //SchemaBuilder.ExecuteSql(unqiueQuery);
            return 5;
        }


        public int UpdateFrom5()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(PAYEReceipt).Name);

            SchemaBuilder.AlterTable(typeof(PAYEReceipt).Name, table => table.DropColumn("UtilizationStatusId"));
            SchemaBuilder.AlterTable(typeof(PAYEReceipt).Name, table => table.DropColumn("UtilizedAmount"));

            string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} bigint NOT NULL", tableName, nameof(PAYEReceipt.Receipt) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ADD UNIQUE({1})", tableName, nameof(PAYEReceipt.Receipt) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(PAYEReceipt).Name, table => table.AddColumn(nameof(PAYEReceipt.UtilizationStatusId), System.Data.DbType.Int32));
            return 7;
        }
    }
}