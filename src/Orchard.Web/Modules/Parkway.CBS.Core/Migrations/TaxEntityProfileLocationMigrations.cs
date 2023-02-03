using Orchard;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using System;
using System.Linq;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Orchard.Logging;


namespace Parkway.CBS.Core.Migrations
{
    public class TaxEntityProfileLocationMigrations : DataMigrationImpl
    {
        private readonly IOrchardServices _orchardServices;
        ILogger Logger { get; set; }
        public TaxEntityProfileLocationMigrations(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;

        }

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxEntityProfileLocation).Name,
                table => table
                            .Column<int>("Id", column => column.Identity().PrimaryKey())
                            .Column<string>(nameof(TaxEntityProfileLocation.Name), column => column.NotNull().WithLength(150))
                            .Column<int>(nameof(TaxEntityProfileLocation.State) + "_Id", column => column.NotNull())
                            .Column<int>(nameof(TaxEntityProfileLocation.LGA) + "_Id", column => column.NotNull())
                            .Column<string>(nameof(TaxEntityProfileLocation.Address), column => column.NotNull().WithLength(300))
                            .Column<Int64>(nameof(TaxEntityProfileLocation.TaxEntity) + "_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );

            string tableName = SchemaBuilder.TableDbName(typeof(TaxEntityProfileLocation).Name);
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint TaxEntityProfileLocation_Unique_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(TaxEntityProfileLocation.Address), nameof(TaxEntityProfileLocation.TaxEntity) + "_Id"));
            SchemaBuilder.ExecuteSql(string.Format("ALTER TABLE[dbo].[{0}] ADD constraint TaxEntityProfileLocation_Name_Unique_Constraint UNIQUE([{1}], [{2}]); ", tableName, nameof(TaxEntityProfileLocation.Name), nameof(TaxEntityProfileLocation.TaxEntity) + "_Id"));

            return 1;
        }


        public int UpdateFrom1()
        {
            try
            {
                Node setting = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                     .Node.Where(k => k.Key == TenantConfigKeys.SiteNameOnFile.ToString()).FirstOrDefault();

                if (setting != null)
                {
                    if (setting.Value == "CBSPolice" || setting.Value.Contains("POSSAP") || setting.Value.Contains("Police"))
                    {
                        string tableOneName = SchemaBuilder.TableDbName(typeof(TaxEntityProfileLocation).Name);
                        string tableTwoName = SchemaBuilder.TableDbName(typeof(TaxEntity).Name);
                        string tableThreeName = SchemaBuilder.TableDbName(typeof(LGA).Name);

                        string queryStringFile = string.Format("INSERT INTO [dbo].[{0}] (Name, State_Id, LGA_Id, Address, TaxEntity_Id, CreatedAtUtc, UpdatedAtUtc) SELECT T1.Recipient, T2.State_Id, T1.StateLGA_Id, T1.Address, T1.Id, GETDATE(), GETDATE() FROM [dbo].[{1}] AS T1 INNER JOIN [dbo].[{2}] AS T2 ON T1.StateLGA_Id = T2.Id;", tableOneName, tableTwoName, tableThreeName);
                        SchemaBuilder.ExecuteSql(queryStringFile);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " SN " + _orchardServices.WorkContext.CurrentSite.SiteName);
                throw;
            }
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntityProfileLocation).Name, table => table.AddColumn(nameof(TaxEntityProfileLocation.IsDefault), System.Data.DbType.Boolean, col => col.Nullable()));
            
            return 3;
        }


        public int UpdateFrom3()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(TaxEntityProfileLocation).Name);
            string queryString = string.Format("UPDATE {0} SET IsDefault = 1", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN IsDefault bit NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            return 4;
        }


        public int UpdateFrom4()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(TaxEntityProfileLocation).Name);
            string queryString = string.Format("ALTER TABLE {0} add [Code] as concat(substring('ABCDEFGHIJKLMNOPQRSTUVWXYZ',case when abs(checksum([Id]))%(26)>=(24) then abs(checksum([Id]))%(26)-(2) else abs(checksum([Id]))%(26)+(1) end,(3)),'-',case when len(CONVERT([nvarchar](50),[Id]))<(5) then '000' else '' end,[Id]) PERSISTED NOT NULL", tableName);

            SchemaBuilder.ExecuteSql(queryString);

            return 5;
        }
    }
}