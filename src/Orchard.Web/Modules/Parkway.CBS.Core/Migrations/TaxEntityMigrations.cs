using Orchard;
using Orchard.Data.Migration;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Utilities;
using Orchard.Logging;

namespace Parkway.CBS.Core.Migrations
{
    public class TaxEntityMigrations : DataMigrationImpl
    {
        private readonly ITenantStateSettings<TenantCBSSettings> _stateSettingsRepo;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }

        public TaxEntityMigrations(ITenantStateSettings<TenantCBSSettings> stateSettingsRepo, IOrchardServices orchardServices)
        {
            _stateSettingsRepo = stateSettingsRepo;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(TaxEntity).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("TaxPayerIdentificationNumber", column => column.WithLength(100).Nullable())
                            .Column<string>("RecipientValue1", column => column.Nullable())
                            .Column<string>("RecipientValue2", column => column.Nullable())
                            .Column<string>("Recipient", column => column.Nullable().WithLength(500))
                            .Column<string>("RCNumber", column => column.Nullable().WithLength(50))
                            .Column<string>("Email", column => column.Nullable())
                            .Column<string>("Address", column => column.Nullable().WithLength(500))
                            .Column<string>("Occupation", column => column.Nullable())
                            .Column<string>("PhoneNumber", column => column.Nullable().WithLength(50))
                            .Column<Int64>("PrimaryContactId", column => column.Nullable())
                            .Column<Int64>("CashflowCustomerId", column => column.Nullable())
                            .Column<int>("TaxEntityType", column => column.NotNull())
                            .Column<int>("TaxEntityCategory_Id", column => column.NotNull())
                            .Column<Int64>("TaxEntityAccount_Id", column => column.Nullable())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                );
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("CompositeUniqueKey", System.Data.DbType.String, column => column.WithLength(500)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.DropColumn("RecipientValue1"));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.DropColumn("RecipientValue2"));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.DropColumn("Occupation"));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("PreviousTIN", System.Data.DbType.String, column => column.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("DOB", System.Data.DbType.DateTime));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("Gender", System.Data.DbType.String, column => column.WithLength(20)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("RegDate", System.Data.DbType.DateTime));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("LGA", System.Data.DbType.String, column => column.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("Ward", System.Data.DbType.String, column => column.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("Occupation", System.Data.DbType.String, column => column.WithLength(500)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("SourceOfIncome", System.Data.DbType.String, column => column.WithLength(500)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("Nationality", System.Data.DbType.String, column => column.WithLength(250)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("State", System.Data.DbType.String, column => column.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("StateOfOrigin", System.Data.DbType.String, column => column.WithLength(100)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("TaxAuthority", System.Data.DbType.String, column => column.WithLength(250)));
            return 3;
        }


        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("OldPayerId", System.Data.DbType.String, column => column.WithLength(250)));

            string tableName = SchemaBuilder.TableDbName(typeof(TaxEntity).Name);

            string queryString = string.Format("ALTER TABLE {0} add [PayerId] as (case when [OldPayerId] IS NULL then concat(substring('ABCDEFGHIJKLMNOPQRSTUVWXYZ',case when abs(checksum([Id]))%(26)=(25) then abs(checksum([Id]))%(26)-(2) else abs(checksum([Id]))%(26)+(1) end,(2)),'-',case when len(CONVERT([nvarchar](50),[Id]))<(5) then '000' else '' end,[Id]) else [OldPayerId] end) PERSISTED NOT NULL", tableName);
            SchemaBuilder.ExecuteSql(queryString);

            string unqiueQuery = string.Format("ALTER TABLE [dbo].[{0}] ADD UNIQUE(PayerId); ", tableName);
            SchemaBuilder.ExecuteSql(unqiueQuery);

            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.DropColumn("CompositeUniqueKey"));
            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("TaxPayerCode", System.Data.DbType.String, column => column.WithLength(250)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("IsDisabled", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("ShortName", System.Data.DbType.String, column => column.WithLength(20)));
            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("UnknownProfile", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 7;
        }


        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("AddedByExternalExpertSystem_Id", System.Data.DbType.Int32, col => col.Nullable()));
            return 8;
        }

        public int UpdateFrom8()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("StateLGA_Id", System.Data.DbType.Int32, col => col.Nullable()));
            return 9;
        }

        public int UpdateFrom9()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("ContactPersonName", System.Data.DbType.String, col => col.Nullable().WithLength(500)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("ContactPersonEmail", System.Data.DbType.String, col => col.Nullable()));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("ContactPersonPhoneNumber", System.Data.DbType.String, col => col.Nullable().WithLength(50)));
            return 10;
        }

        public int UpdateFrom10()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("BVN", System.Data.DbType.String, col => col.Nullable().WithLength(50)));
            return 11;
        }

        public int UpdateFrom11()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("BusinessType", System.Data.DbType.Int32, col => col.Nullable()));
            return 12;
        }

        public int UpdateFrom12()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("IdentificationType", System.Data.DbType.Int32, col => col.Nullable()));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn("IdentificationNumber", System.Data.DbType.String, column => column.Nullable().WithLength(20)));
            return 13;
        }


        public int UpdateFrom13()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(TaxEntity).Name);
            IEnumerable<TenantCBSSettings> result = null;
            //get tenant default LGA Id
            try
            {
                result = _stateSettingsRepo.GetCollection(t => t.Id != 0);
                //will throw error if a new tenant, the tenant settings table might not have been created before this query
            }
            catch (Exception) { }

            string queryString = string.Empty;

            if (result != null && result.Any())
            {
                queryString = string.Format("UPDATE {0} SET [StateLGA_Id] = {1}, [UpdatedAtUtc] = CURRENT_TIMESTAMP WHERE StateLGA_Id IS NULL", tableName, result.Single().DefaultLGA.Id);
                SchemaBuilder.ExecuteSql(queryString);
            }

            queryString = string.Format("ALTER TABLE {0} ALTER COLUMN {1} int NOT NULL", tableName, nameof(TaxEntity.StateLGA) + "_Id");
            SchemaBuilder.ExecuteSql(queryString);

            return 14;
        }


        public int UpdateFrom14()
        {
            try
            {
                Node setting = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                     .Node.Where(k => k.Key == TenantConfigKeys.SiteNameOnFile.ToString()).FirstOrDefault();

                if (setting != null)
                {
                    if (setting.Value == "CBSPolice" || setting.Value.Contains("POSSAP") || setting.Value.Contains("Police"))
                    {
                        string tableName = SchemaBuilder.TableDbName(typeof(TaxEntity).Name);
                        string queryString = string.Format("CREATE UNIQUE INDEX CHECK_FOR_DUPLICATE_TIN_IF_NOT_NULL ON [dbo].[{0}]({1}) WHERE {1} IS NOT NULL ", tableName, nameof(TaxEntity.TaxPayerIdentificationNumber));
                        SchemaBuilder.ExecuteSql(queryString);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " SN " + _orchardServices.WorkContext.CurrentSite.SiteName);
                throw;
            }

            return 15;
        }

        public int UpdateFrom15()
        {
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn(nameof(TaxEntity.FirstName), System.Data.DbType.String, column => column.Nullable().WithLength(500)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn(nameof(TaxEntity.MiddleName), System.Data.DbType.String, column => column.Nullable().WithLength(500)));
            SchemaBuilder.AlterTable(typeof(TaxEntity).Name, table => table.AddColumn(nameof(TaxEntity.LastName), System.Data.DbType.String, column => column.Nullable().WithLength(500)));
            return 16;
        }
    }
}