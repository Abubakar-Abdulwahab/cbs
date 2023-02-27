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
    public class RequireLoginUserMigrations : DataMigrationImpl
    {

        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }

        public RequireLoginUserMigrations(IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CBSUser).Name,
                table => table
                            .Column<Int64>("Id", column => column.PrimaryKey().Identity())
                            .Column<string>("Name", column => column.NotNull().WithLength(250))
                            .Column<int>("UserPartRecord_Id", column => column.NotNull().Unique())
                            .Column<Int64>("TaxEntity_Id", column => column.NotNull())
                            .Column<DateTime>("CreatedAtUtc", column => column.NotNull())
                            .Column<DateTime>("UpdatedAtUtc", column => column.NotNull())
                ).AlterTable(typeof(CBSUser).Name,
                    table => table.CreateIndex("GET_USER", new string[] { "UserPartRecord_Id" }));
            return 1;
        }


        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(CBSUser).Name, table => table.AddColumn("Verified", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 2;
        }


        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(CBSUser).Name, table => table.AddColumn("PhoneNumber", System.Data.DbType.String, column => column.Nullable().WithLength(11)));
            SchemaBuilder.AlterTable(typeof(CBSUser).Name, table => table.AddColumn("Email", System.Data.DbType.String, column => column.Nullable()));
            SchemaBuilder.AlterTable(typeof(CBSUser).Name, table => table.AddColumn("Address", System.Data.DbType.String, column => column.Nullable().WithLength(500)));
            SchemaBuilder.AlterTable(typeof(CBSUser).Name, table => table.AddColumn("IsAdministrator", System.Data.DbType.Boolean, column => column.WithDefault(false)));
            return 3;
        }


        public int UpdateFrom3()
        {
            string tableName = SchemaBuilder.TableDbName(typeof(CBSUser).Name);
            string referencedTable = SchemaBuilder.TableDbName(typeof(TaxEntity).Name);

            string queryString = string.Format("UPDATE cbsuser SET cbsUser.PhoneNumber = taxEnt.PhoneNumber, cbsUser.Email = taxEnt.Email, cbsUser.Address = taxEnt.Address, cbsUser.IsAdministrator = 1 FROM {0} cbsuser INNER JOIN {1} AS taxEnt ON cbsuser.TaxEntity_Id = taxEnt.Id;", tableName, referencedTable);
            SchemaBuilder.ExecuteSql(queryString);

            return 4;
        }


        public int UpdateFrom4()
        {
            try
            {
                Node setting = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName)
                     .Node.Where(k => k.Key == TenantConfigKeys.SiteNameOnFile.ToString()).FirstOrDefault();

                if (setting != null)
                {
                    if (setting.Value == "CBSPolice" || setting.Value.Contains("POSSAP") || setting.Value.Contains("Police"))
                    {
                        string tableName = SchemaBuilder.TableDbName(typeof(CBSUser).Name);

                        string queryString = string.Format("ALTER TABLE {0} ALTER COLUMN PhoneNumber nvarchar(11) NOT NULL", tableName);
                        SchemaBuilder.ExecuteSql(queryString);

                        queryString = string.Format("ALTER TABLE {0} ALTER COLUMN Email nvarchar(255) NOT NULL", tableName);
                        SchemaBuilder.ExecuteSql(queryString);

                        queryString = string.Format("ALTER TABLE {0} ALTER COLUMN Address nvarchar(500) NOT NULL", tableName);
                        SchemaBuilder.ExecuteSql(queryString);

                        queryString = string.Format("ALTER TABLE {0} ALTER COLUMN IsAdministrator bit NOT NULL", tableName);
                        SchemaBuilder.ExecuteSql(queryString);
                    }
                }

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " SN " + _orchardServices.WorkContext.CurrentSite.SiteName);
                throw;
            }
            return 5;
        }


        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(CBSUser).Name, table => table.AddColumn(nameof(CBSUser.IsActive), System.Data.DbType.Boolean, column => column.NotNull().WithDefault(true)));
            return 6;
        }
    }
}