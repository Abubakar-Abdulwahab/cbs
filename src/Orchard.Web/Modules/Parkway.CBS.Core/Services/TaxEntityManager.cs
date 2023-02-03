using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.DataFilters.AssessmentReport.SearchFilters;
using Parkway.CBS.Core.DataFilters.TaxPayerReport;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Migrations;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.FileUpload.BModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace Parkway.CBS.Core.Services
{
    public class TaxEntityManager : BaseManager<TaxEntity>, ITaxEntityManager<TaxEntity>
    {
        private readonly IRepository<TaxEntity> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public TaxEntityManager(IRepository<TaxEntity> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get the list of tax payers with their cash flow credentials
        /// </summary>
        /// <param name="savedAndUnSavedTaxPayersList"></param>
        /// <returns>IList{CashFlowCustomerTaxPayerModel}</returns>
        public IList<CashFlowCustomerTaxPayerModel> CashFlowTaxPayersModelForTaxPayersThatHaveBeenSaved(List<TaxEntity> savedAndUnSavedTaxPayers)
        {
            var session = _transactionManager.GetSession();
            var listOfTaxPayerIds = savedAndUnSavedTaxPayers.Select(txp => txp.TaxPayerIdentificationNumber).ToArray();

            return session.CreateCriteria<TaxEntity>().SetProjection(Projections.ProjectionList()
               .Add(Projections.Property("Id"), "TaxEntityId")
               .Add(Projections.Property("TaxPayerIdentificationNumber"), "IdentificationNumber")
               .Add(Projections.Property("PrimaryContactId"), "PrimaryContactId")
               .Add(Projections.Property("CashflowCustomerId"), "CashFlowId"))
               .Add(Restrictions.In("TaxPayerIdentificationNumber", listOfTaxPayerIds))
               .SetResultTransformer(Transformers.AliasToBean<CashFlowCustomerTaxPayerModel>()).List<CashFlowCustomerTaxPayerModel>();
            //    public Int64 TaxEntityId { get; set; }
            //public string IdentificationNumber { get; set; }
            //public Int64 PrimaryContactId { get; set; }
            //public Int64 CashFlowId { get; set; }
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="batchNumber"></param>
        ///// <param name="revenueHeadId"></param>
        ///// <param name="billingId"></param>
        ///// <returns></returns>
        //public IList<RefDataTemp> GetRefDataWithoutACorrespondingTaxEntityRecord(string batchNumber, int revenueHeadId, int billingId)
        //{
        //    var session = _transactionManager.GetSession();
        //    //same execution plan but different reads and latency, the latter query over performs better
        //    var subquery = QueryOver.Of<TaxEntity>().Select(x => x.TaxPayerIdentificationNumber);
        //    var refDataTemp = session.QueryOver<RefDataTemp>().Where(rf => (rf.BatchNumber == batchNumber) && (rf.RevenueHeadId == revenueHeadId) && (rf.BillingModelId == billingId)).WithSubquery.WhereProperty(rf => rf.TaxIdentificationNumber).NotIn<TaxEntity>(subquery)
        //        .List();

        //    var subquery2 = QueryOver.Of<TaxEntity>().Select(x => x.TaxPayerIdentificationNumber).DetachedCriteria;
        //    var refDataTemp2 = session.QueryOver<RefDataTemp>().Where(rf => (rf.BatchNumber == batchNumber) && (rf.RevenueHeadId == revenueHeadId) && (rf.BillingModelId == billingId)).Where(Subqueries.NotExists(subquery2)).List();

        //    return refDataTemp;
        //}


        /// <summary>
        /// Gets the joiner between tax entities and ref data
        /// </summary>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        /// <returns>IList{RefDataAndCashflowDetails} <see cref="RefDataAndCashflowDetails"/> ProcessResponseModel | if error occurs exception message is added to the return type</returns>
        public ProcessResponseModel GetRefDataTempJoinerWithTaxIdentification(string batchNumber, int revenueHeadId, int billingId)
        {
            var session = _transactionManager.GetSession();
            try
            {
                var queryString = string.Format(@"SELECT t.CashflowCustomerId as CashflowCustomerId, t.PrimaryContactId as CashflowPrimaryContactId, r.TaxIdentificationNumber as TaxIdentificationNumber, r.Recipient as Recipient, r.Address as Address, r.TaxEntityCategoryId as TaxEntityCategoryId, r.Email as Email, r.AdditionalDetails as AdditionalDetails, r.Amount as Amount FROM [{0}] as t RIGHT JOIN [{1}] as r ON t.TaxPayerIdentificationNumber = r.TaxIdentificationNumber WHERE r.BatchNumber = '{2}' AND r.RevenueHeadId = '{3}' AND r.BillingModelId = '{4}'", "Parkway_CBS_Core_TaxEntity", "Parkway_CBS_Core_RefDataTemp", batchNumber, revenueHeadId.ToString(), billingId.ToString());

                return new ProcessResponseModel { MethodReturnObject = session.CreateSQLQuery(queryString).SetResultTransformer(Transformers.AliasToBean<RefDataAndCashflowDetails>()).List<RefDataAndCashflowDetails>() };
            }
            catch (Exception exception)
            {
                Logger.Error("Error getting joiner of tax entity and ref data temp", exception);
                return new ProcessResponseModel { HasErrors = true, ErrorMessage = string.Format("Exception: {0} StackTrace: {1}", exception.Message, exception.StackTrace) };
            }
        }

        /// <summary>
        /// Get the tax payers for the specified TIN or Phone number
        /// <para>Searches for tax entities with either TIN or Phone number depending on column name</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="columnName"></param>
        /// <returns>List{TaxPayersWithDetails}</returns>
        public List<TaxPayerWithDetails> GetListOfTaxPayersWithDetails(string value, string columnName)
        {
            switch (columnName)
            {
                case "SearchByTIN":
                    return _transactionManager.GetSession().Query<TaxEntity>().Where(txp => txp.TaxPayerIdentificationNumber == value.Trim())
                        .Select(txp => new TaxPayerWithDetails { Address = txp.Address, Category = txp.TaxEntityCategory.Name, Name = txp.Recipient, Id = txp.Id, PhoneNumber = txp.PhoneNumber, TIN = value.Trim(), Email = txp.Email }).ToList();

                case "SearchByPhoneNumber":
                    return _transactionManager.GetSession().Query<TaxEntity>().Where(txp => txp.PhoneNumber == value.Trim())
                        .Select(txp => new TaxPayerWithDetails { Address = txp.Address, Category = txp.TaxEntityCategory.Name, Name = txp.Recipient, Id = txp.Id, PhoneNumber = txp.PhoneNumber, TIN = value.Trim() }).ToList();

                default:
                    throw new Exception("Column name not supported");
            }
        }


        /// <summary>
        /// Get tax payer details
        /// </summary>
        /// <param name="id"></param>
        /// <returns>TaxPayerWithDetails</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public TaxPayerWithDetails GetTaxPayerWithDetails(Int64 id)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(txp => txp.Id == id)
                        .Select(txp => new TaxPayerWithDetails { Address = txp.Address, Category = txp.TaxEntityCategory.Name, Name = txp.Recipient, Id = txp.Id, PhoneNumber = txp.PhoneNumber, TIN = txp.TaxPayerIdentificationNumber, PayerId = txp.PayerId, Email = txp.Email, CategoryId = txp.TaxEntityCategory.Id }).SingleOrDefault();
        }

        /// <summary>
        /// Get tax payer category Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>int</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public int GetTaxPayerCtegoryId(Int64 id)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(txp => txp.Id == id)
                        .Select(txp => txp.TaxEntityCategory.Id).SingleOrDefault();
        }


        /// <summary>
        /// search for taxpayer using the specified search params
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Category"></param>
        /// <param name="RCNumber"></param>
        /// <param name="tin"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public IEnumerable<TaxEntity> TaxPayerSearch(string Name, string Category, string RCNumber, string tin, DateTime StartDate, DateTime EndDate, int take, int skip)
        {
            try
            {
                Logger.Debug("About to fetch tax entity records based on the search params");
                var ReportSession = _transactionManager.GetSession();
                var startDate = Convert.ToDateTime(StartDate).Date;
                var endDate = Convert.ToDateTime(EndDate).Date;

                Logger.Debug("Create nhiv=bernate criteria");
                //var tinCriteria = ReportSession.CreateCriteria<Applicant>("applicant");
                var taxEntityCriteria = ReportSession.CreateCriteria<TaxEntity>("tin");
                taxEntityCriteria.CreateAlias("tin.TaxEntityCategory", "taxEntityCat");

                taxEntityCriteria.Add(Restrictions.Disjunction()
                        .Add(Restrictions.InsensitiveLike("Recipient", Name.Trim(), MatchMode.Anywhere))
                        .Add(Restrictions.InsensitiveLike("TaxPayerIdentificationNumber", tin.Trim(), MatchMode.Anywhere))
                        .Add(Restrictions.InsensitiveLike("RCNumber", RCNumber.Trim(), MatchMode.Anywhere))
                        .Add(Restrictions.InsensitiveLike("taxEntityCat.Name", Category.Trim(), MatchMode.Anywhere)));

                if (StartDate == null && EndDate != null)
                {
                    taxEntityCriteria.Add(Restrictions.Le("UpdatedAtUtc", endDate));
                }
                if (StartDate != null && EndDate == null)
                {
                    taxEntityCriteria.Add(Restrictions.Ge("CreatedAtUtc", startDate));
                }
                if (StartDate != null && EndDate != null)
                {
                    //taxEntityCriteria.Add(Restrictions.Between("DateOfRegistration", startDate, endDate));
                    taxEntityCriteria.Add(Restrictions.Ge("CreatedAtUtc", startDate));
                    taxEntityCriteria.Add(Restrictions.Le("UpdatedAtUtc", endDate));
                }
                //tinCriteria.Add(Restrictions.InsensitiveLike("TIN", tin.Trim(), MatchMode.Anywhere));

                var applicantSearch = taxEntityCriteria.List<TaxEntity>().Skip(skip).Take(take).ToList();
                //var applicantSearch = tinCriteria.List<Applicant>();
                Logger.Debug($"successfully fetched {applicantSearch.Count()} tax entity records");

                return applicantSearch;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not successfully fetch tax entity records");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }


        public void SaveFromScrapFile(List<ScrapFileModel> scraps)
        {
            Logger.Information("Saving direct assessment payee records for batch id ");
            //save entities into temp table
            int chunkSize = 500000;
            var dataSize = scraps.Count;
            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;
            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;
            //var startTime = Stopwatch.StartNew();
            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable("Parkway_CBS_Core_" + typeof(TaxEntity).Name);
                    dataTable.Columns.Add(new DataColumn("TaxPayerIdentificationNumber", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Recipient", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("RCNumber", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Email", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Address", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Occupation", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("PhoneNumber", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("TaxEntityCategory_Id", typeof(int)));
                    //dataTable.Columns.Add(new DataColumn("TaxEntityAccount_Id", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("CompositeUniqueKey", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("PreviousTIN", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("DOB", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("Gender", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("RegDate", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("Address", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("LGA", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Ward", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("SourceOfIncome", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("Nationality", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("State", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("StateOfOrigin", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("TaxAuthority", typeof(string)));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));
                    scraps.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["TaxPayerIdentificationNumber"] = x.TIN;

                        row["Recipient"] = x.CombinedName;
                        //row["RCNumber"] = x..TaxPayerTIN.Value;
                        row["Email"] = x.Email;
                        row["Address"] = x.Address;
                        row["Occupation"] = string.IsNullOrEmpty(x.Occupation) ? null : x.Occupation;
                        row["PhoneNumber"] = x.Phone;
                        row["Email"] = x.Email;
                        row["PhoneNumber"] = x.Phone;
                        row["TaxEntityCategory_Id"] = 1;
                        //row["TaxEntityAccount_Id"] = x.TaxAccount;
                        row["CompositeUniqueKey"] = string.Format("{0}{1}", x.Phone, 1);
                        row["PreviousTIN"] = x.Prev_TIN;
                        row["DOB"] = x.BOD.HasValue ? (object)x.BOD.Value : DBNull.Value;
                        row["Gender"] = x.Gender;
                        row["RegDate"] = x.RegDate.HasValue ? (object)x.RegDate.Value : DBNull.Value;
                        row["LGA"] = x.Lga;
                        row["Ward"] = x.Ward;
                        row["SourceOfIncome"] = x.Source;
                        row["Nationality"] = x.Nationality;
                        row["State"] = x.State;
                        row["StateOfOrigin"] = x.StateO;
                        row["TaxAuthority"] = x.TaxAuth;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    Logger.Information(string.Format("Insertion for direct assessment batch payee records  has started Size: {0} Skip: {1}", dataSize, skip));
                    if (!SaveBundle(dataTable, "Parkway_CBS_Core_" + typeof(TaxEntity).Name))
                    { throw new Exception("Error saving excel file details for batch Id "); }
                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message); throw;
            }
            Logger.Information(string.Format("SIZE: {0}", dataSize));
        }


        //private List<TaxEntity> GetTaxPayersRecordWithCategory(int page, int pageSize, string searchParams, long taxCategoryId)
        //{
        //    Logger.Information("Getting Tax payer records ");
        //    var session = _transactionManager.GetSession();

        //    var taxEntityCriteria = session.CreateCriteria<TaxEntity>("taxEntity");

        //    taxEntityCriteria.Add(Restrictions.Disjunction()
        //        .Add(Restrictions.InsensitiveLike("Recipient", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere))
        //        .Add(Restrictions.InsensitiveLike("TaxPayerIdentificationNumber", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere))
        //        .Add(Restrictions.InsensitiveLike("PhoneNumber", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere))
        //        .Add(Restrictions.InsensitiveLike("Email", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere))
        //        .Add(Restrictions.InsensitiveLike("Address", String.IsNullOrEmpty(searchParams) ? "" : searchParams, MatchMode.Anywhere)));

        //    var taxEntities = taxEntityCriteria.List<TaxEntity>().Skip(page).Take(pageSize).Where(c => c.TaxEntityCategory.Id == taxCategoryId).ToList();

        //    Logger.Information($"Successfully retrieved {taxEntities.Count()} tax entites");

        //    return taxEntities;
        //}


        public List<TaxPayerWithDetails> GetListOfTaxPayersWithCategoryId(int categoryId)
        {
            return _transactionManager.GetSession().Query<TaxEntity>()
                .Where(txp => txp.TaxEntityCategory == new TaxEntityCategory { Id = categoryId })
                .Select(txp => new TaxPayerWithDetails { Address = txp.Address.ToLower(), Name = txp.Recipient.ToUpper(), Id = txp.Id }).OrderBy(k => k.Name).ToList();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns></returns>
        public TaxPayerWithDetails GetTaxPayerWithDetails(string payerId)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(txp => txp.PayerId == payerId)
                        .Select(txp => new TaxPayerWithDetails { Address = txp.Address, Category = txp.TaxEntityCategory.Name, Name = txp.Recipient, Id = txp.Id, PhoneNumber = txp.PhoneNumber, TIN = txp.TaxPayerIdentificationNumber, Email = txp.Email, CategoryId = txp.TaxEntityCategory.Id, PayerId = txp.PayerId, TaxPayerCode = txp.TaxPayerCode, CreatedAtUtc = txp.CreatedAtUtc, CategoryStringIdentifier = txp.TaxEntityCategory.StringIdentifier }).SingleOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateTaxPayer(TaxPayerWithDetails model)
        {
            try
            {
               var taxEntity = _transactionManager.GetSession().Query<TaxEntity>().FirstOrDefault(txp => txp.PayerId == model.PayerId);
                taxEntity.TaxPayerCode = model.TaxPayerCode;
                _repository.Update(taxEntity);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                return false;
            }
        }

        public bool CheckIfTaxPayerCodeExist(string taxPayerCode, string taxPayerId)
        {
            try
            {
                var taxEntity = _transactionManager.GetSession().Query<TaxEntity>().FirstOrDefault(txp => txp.TaxPayerCode == taxPayerCode && txp.PayerId != taxPayerId);
              
                if(taxEntity != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Get tax entity by tax payer code
        /// <para>returns only limited properties</para>
        /// </summary>
        /// <param name="taxPayerCode"></param>
        /// <returns>TaxPayerWithDetails</returns>
        public TaxPayerWithDetails GetTaxPayerDetailsByTaxPayerCode(string taxPayerCode)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(txp => txp.TaxPayerCode == taxPayerCode)
                        .Select(txp => new TaxPayerWithDetails { CategoryId = txp.TaxEntityCategory.Id, PayerId = txp.PayerId, Id = txp.Id }).SingleOrDefault();
        }


        /// <summary>
        /// Get tax payer details
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>TaxPayerWithDetails</returns>
        public TaxPayerWithDetails GetTaxPayerDetailsByPhoneNumber(string phoneNumber)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(txp => txp.PhoneNumber == phoneNumber)
                        .Select(txp => new TaxPayerWithDetails { Address = txp.Address, Name = txp.Recipient, PhoneNumber = txp.PhoneNumber, PayerId = txp.PayerId, Email = txp.Email }).FirstOrDefault();
        }

        /// <summary>
        /// Get tax profile category Id
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns>int</returns>
        public int GetTaxPayerCategoryId(string payerId)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(txp => txp.PayerId == payerId)
                        .Select(txp => txp.TaxEntityCategory.Id).Single();
        }


        /// <summary>
        /// Get the identity type Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>int</returns>
        public int GetIdentityType(long id)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(txp => txp.Id == id)
                       .Select(txp => txp.IdentificationType).Single();
        }


        /// <summary>
        /// Returns Only the tax entity Id
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>The Id of the tax entity</returns>
        /// <exception cref="Exception"> Thrown when no record is found matching the query</exception>
        public long GetTaxEntityId(Expression<Func<TaxEntity, bool>> lambda)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(lambda)
                        .Select(txp => txp.Id).Single();
        }

        /// <summary>
        /// Gets tax entity details with specified payer id
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns></returns>
        public TaxEntityViewModel GetTaxEntityDetailsWithPayerId(string payerId)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(x => x.PayerId == payerId).Select(x => new TaxEntityViewModel
            {
                Id = x.Id,
                Recipient = x.Recipient,
                PhoneNumber = x.PhoneNumber,
                CategoryName = x.TaxEntityCategory.Name,
                Email = x.Email,
                Address = x.Address,
                PayerId = x.PayerId,
                IdType = x.IdentificationType,
                IdNumber = x.IdentificationNumber
            }).SingleOrDefault();
        }


        /// <summary>
        /// Get the identification type for this Id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns>KeyValuePair{int, string}</returns>
        public KeyValuePair<int, string> GetIdentificationTypeAndValue(long id)
        {
            return _transactionManager.GetSession().Query<TaxEntity>().Where(t => t.Id == id)
                        .Select(txp => new KeyValuePair<int, string>(txp.IdentificationType, txp.IdentificationNumber)).Single();
        }
    }
}