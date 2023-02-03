using System;
using System.Linq;
using Orchard;
using Orchard.Data.Migration;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;

namespace Parkway.CBS.Core.Migrations
{
    public class ReceiptMigration : DataMigrationImpl
    {
        private readonly IOrchardServices _orchardServices;

        public ReceiptMigration(IOrchardServices orchardServices) { _orchardServices = orchardServices; }

        public int Create()
        {
            Node setting = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
               .Node.Where(k => k.Key == TenantConfigKeys.ReceiptPrefix.ToString()).FirstOrDefault();

            string tableName = SchemaBuilder.TableDbName(typeof(Receipt).Name);

            SchemaBuilder.CreateTable(typeof(Receipt).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                .Column<string>("OldReceiptNumber", column => column.Nullable())
                .Column<Int64>("Invoice_Id", column => column.NotNull())
                .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string queryString = string.Format("ALTER TABLE {0} add [ReceiptNumber] as ((case when [OldReceiptNumber] IS NULL then concat('{1}',datepart(dayofyear,[CreatedAtUtc]),'-',datepart(month,[CreatedAtUtc]),'-',case when len(CONVERT([nvarchar](50),[Id]))<(5) then '00000' else '' end,[Id]) else [OldReceiptNumber] end)) PERSISTED", tableName, setting.Value);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }


        public int UpdateFrom1()
        {
            return 2;
        }


        public int UpdateFrom2()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(Receipt).Name);
            string queryString = string.Format("CREATE NONCLUSTERED INDEX RECEIPT_NUMBER_INDEX ON dbo.{0}(ReceiptNumber)", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 3;
        }

        public int UpdateFrom3()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(Receipt).Name);
            string queryString = string.Format("ALTER TABLE {0} ADD UNIQUE(ReceiptNumber)", tableName);
            SchemaBuilder.ExecuteSql(queryString);
            return 4;
        }

    }
}