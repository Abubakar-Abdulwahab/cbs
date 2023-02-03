using Orchard;
using Orchard.Data.Migration;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Migrations
{
    public class PAYEBatchItemReceiptMigrations : DataMigrationImpl
    {

        private readonly IOrchardServices _orchardServices;

        public PAYEBatchItemReceiptMigrations(IOrchardServices orchardServices) { _orchardServices = orchardServices; }

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

            SchemaBuilder.CreateTable(typeof(PAYEBatchItemReceipt).Name,
                table => table
                    .Column<Int64>(nameof(PAYEBatchItemReceipt.Id), column => column.PrimaryKey().Identity())
                    .Column<Int64>(nameof(PAYEBatchItemReceipt.PAYEBatchItem) + "_Id", column => column.NotNull())
                    .Column<DateTime>(nameof(PAYEBatchItemReceipt.CreatedAtUtc), column => column.NotNull())
                    .Column<DateTime>(nameof(PAYEBatchItemReceipt.UpdatedAtUtc), column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(PAYEBatchItemReceipt).Name);
            string queryString = string.Format("ALTER TABLE {0} ADD [ReceiptNumber] as (concat('{1}','-')+case when len(rtrim(CONVERT([nvarchar](20),[Id],(0))))>(7) then CONVERT([nvarchar](20),[Id],(0)) else right(replicate((0),(7))+rtrim(CONVERT([nvarchar](7),[Id],(0))),(7)) end) PERSISTED;", tableName, setting.Value);

            queryString += string.Format("CREATE NONCLUSTERED INDEX PAYE_RECEIPT_NUMBER_INDEX ON dbo.{0}(ReceiptNumber)", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 1;
        }

    }
}