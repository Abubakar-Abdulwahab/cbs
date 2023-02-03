using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Exceptions;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Utilities;
using System.Threading.Tasks;
using Orchard.Environment.ShellBuilders;
using Autofac;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Parkway.CBS.Core.Lang;
using System.Text;
using System.Security.Cryptography;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreTaxPayerService : CoreBaseService, ICoreTaxPayerService
    {
        private readonly IOrchardServices _orchardServices;
        public IInvoicingService _invoicingService;
        private readonly ICoreRevenueHeadService _revenueHeadCoreService;
        public Localizer T { get; set; }
        private readonly IInvoiceManager<Invoice> _invoiceRepository;
        private readonly ITaxEntityManager<TaxEntity> _taxPayerRepository;
        private readonly ITaxEntityAccountManager<TaxEntityAccount> _taxPayerAccountRepository;
        private readonly Lazy<ITaxEntityCategoryManager<TaxEntityCategory>> _taxEntityCategoryRepository;

        public CoreTaxPayerService(IOrchardServices orchardServices,
            IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider,
            IInvoicingService invoicingService, ICoreRevenueHeadService revenueHeadCoreService, IInvoiceManager<Invoice> invoiceRepository, ITaxEntityManager<TaxEntity> taxPayerRepository, ITaxEntityAccountManager<TaxEntityAccount> taxPayerAccountRepository, Lazy<ITaxEntityCategoryManager<TaxEntityCategory>> taxEntityCategoryRepository)
            : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            _invoicingService = invoicingService;
            _revenueHeadCoreService = revenueHeadCoreService;
            Logger = NullLogger.Instance;
            _invoiceRepository = invoiceRepository;
            _taxPayerRepository = taxPayerRepository;
            _taxPayerAccountRepository = taxPayerAccountRepository;
            _taxEntityCategoryRepository = taxEntityCategoryRepository;
        }

        /// <summary>
        /// Validate and save tax profile
        /// <para>Throw CannotSaveTaxEntityException if the model could not be saved, no valid category, phone number or TIN</para>
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="category"></param>
        /// <returns>TaxEntity</returns>
        /// <exception cref="CannotSaveTaxEntityException"></exception>
        public TaxEntityProfileHelper ValidateAndSaveTaxEntity(TaxEntity entity, TaxEntityCategory category)
        {
            if (category == null || category.Id <= 0) { throw new CannotSaveTaxEntityException("No valid category added."); }

            if (entity == null) { throw new CannotSaveTaxEntityException("No valid tax model added."); }

            //at this point this user does not have a tax profile lets create one
            entity.TaxEntityCategory = category;

            TaxEntityAccount account = new TaxEntityAccount { };
            if (!_taxPayerAccountRepository.Save(account))
            {
                _taxPayerAccountRepository.RollBackAllTransactions();
                throw new CannotSaveTaxEntityException(string.Format("Could not save tax record {0}", entity.Recipient));
            }
            entity.TaxEntityAccount = account;
            if (!_taxPayerRepository.Save(entity))
            {
                _taxPayerAccountRepository.RollBackAllTransactions();
                throw new CannotSaveTaxEntityException(string.Format("Could not save tax record {0}", entity.Recipient));
            }
            _taxPayerRepository.Evict(entity);
            entity = _taxPayerRepository.Get(e => e.Id == entity.Id);
            if (category.RequiresLogin)
            {
                return new TaxEntityProfileHelper { RequiresLogin = true, Message = ErrorLang.createdtaxentityrequiredlogin(entity.PayerId, category.Name).ToString(), NewProfile = true, TaxEntity = entity, Category = category };
            }
            return new TaxEntityProfileHelper { Message = Lang.Lang.taxentitycreated(entity.PayerId).ToString(), NewProfile = true, TaxEntity = entity, Category = category };
        }

        /// <summary>
        /// Check that this category is valid
        /// </summary>
        /// <returns>bool</returns>
        public bool CategoryExists(int categoryId)
        {
            return _taxEntityCategoryRepository.Value.Count(c => c.Id == categoryId) == 1 ? true : false;
        }

        /// <summary>
        /// Generate payer Id
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string</returns>
        public string GetPayerId(Int64 idValue)
        {
            string value = Util.ZeroPadUp(idValue.ToString(), 6);
            StringBuilder payerIdBuilder = new StringBuilder();
            string alphabets = "EGPBLUSIHAJCFKDMVWQRZNTXYO";
            int rand1 = 0;
            //int tens = (value % (100));
            using (RandomNumberGenerator cryptoRandomDataGenerator = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[100];
                cryptoRandomDataGenerator.GetBytes(buffer);
                rand1 = buffer[new Random().Next(0, DateTime.Now.Second)];
            }

            int firstCharIndex = (rand1 % 25);
            int secondCharIndex = firstCharIndex + new Random().Next(0, firstCharIndex);
            if (secondCharIndex >= 25) { secondCharIndex -= 25; }
            payerIdBuilder.Append(alphabets.Substring(firstCharIndex, 1));
            payerIdBuilder.Append(alphabets.Substring(secondCharIndex, 1));
            return payerIdBuilder.Append(string.Format("-{0}", value)).ToString();
        }


        /// <summary>
        /// Get the identity type Id for this tax entity Id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public int GetIdentityType(long id)
        {
            try
            {
                return _taxPayerRepository.GetIdentityType(id);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        public TaxEntity GetTaxEntity(Expression<Func<TaxEntity, bool>> lambda)
        {
            try
            {
                return _taxPayerRepository.Get(lambda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Returns Only the tax entity Id
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>The Id of the tax entity</returns>
        /// <exception cref="NoRecordFoundException"> Thrown when no record is found matching the query</exception>
        public long GetTaxEntityId(Expression<Func<TaxEntity, bool>> lambda)
        {
            try
            {
                return _taxPayerRepository.GetTaxEntityId(lambda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new NoRecordFoundException();
            }
        }

        /// <summary>
        /// Get entity by tax payer code
        /// <para>returns limited object props</para>
        /// </summary>
        /// <param name="taxPayerCode"></param>
        /// <returns>TaxPayerWithDetails</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public TaxPayerWithDetails GetTaxEntityDetails(string taxPayerCode)
        {
            return _taxPayerRepository.GetTaxPayerDetailsByTaxPayerCode(taxPayerCode);
        }

        public IEnumerable<TaxEntity> GetTaxEntities(Expression<Func<TaxEntity, bool>> lambda)
        {
            try
            {
                return _taxPayerRepository.GetCollection(lambda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get count of enitities
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns>int</returns>
        public int CheckCountCount(Expression<Func<TaxEntity, bool>> lambda)
        {
            try
            {
                return _taxPayerRepository.Count(lambda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        private async Task<CashflowCustomerAndTempData> CreateCashflowCustomer(RefDataTemp refTempData, string key, ExpertSystemSettings tenant)
        {
            var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", key } });
            var invoiceService = _invoicingService.InvoiceService(context);
            var obj = new CashflowCustomerAndTempData { RefDataTemp = refTempData };
            try
            {
                await invoiceService.CreateCustomerAndInvoiceAsync(new CashFlowCreateCustomerAndInvoice
                {
                    CreateCustomer = new CashFlowCreateCustomer
                    {
                        Address = refTempData.Address,
                        CountryID = tenant.TenantCBSSettings.CountryId,
                        Identifier = refTempData.TaxIdentificationNumber,
                        Name = refTempData.Recipient,
                        StateID = tenant.TenantCBSSettings.StateId,
                        PryContact = new CashFlowCreateCustomer.Contact
                        {
                            Name = refTempData.Recipient,
                            Email = refTempData.Email,
                        }
                    }
                });
                obj.Successful = true;
                return obj;
            }
            catch (Exception exception)
            {
                var message = string.Format("Error calling cashflow to create customers {0} {1}", Util.SimpleDump(refTempData), exception.Message);
                Logger.Error(exception, message);
                return new CashflowCustomerAndTempData { RefDataTemp = refTempData, CashflowCustomer = null, ReasonForFailure = message };
            }
        }

        public async Task<IEnumerable<CashflowCustomerAndTempData>> GetUsersAsync(string key, List<RefDataTemp> taxEntities, ExpertSystemSettings tenant)
        {
            var listOfCashflowCustomers = new List<Task<CashflowCustomerAndTempData>>();

            foreach (RefDataTemp entity in taxEntities)
            {
                listOfCashflowCustomers.Add(CreateCashflowCustomer(entity, key, tenant));
            }
            return await Task.WhenAll(listOfCashflowCustomers);
        }

        public IEnumerable<CashflowCustomerAndTempData> CreateCustomersOnCashFlow(string key, List<RefDataTemp> taxEntities, ExpertSystemSettings tenant)
        {
            return GetUsersAsync(key, taxEntities, tenant).Result;
        }

        /// <summary>
        /// Get distinct ref data records and thier duplicates if any
        /// </summary>
        /// <param name="entitesWithoutCashflowRecord"></param>
        /// <returns>ProcessResponseModel | if no errors <see cref="RefDataDistinctGroupModel"/>RefDataDistinctGroupModel</returns>
        public ProcessResponseModel GetDistinctRefDataRecordsItemsAndDuplicates(ConcurrentStack<RefDataAndCashflowDetails> entitesWithoutCashflowRecord)
        {
            try
            {
                ConcurrentDictionary<string, RefDataAndCashflowDetails> distinctItems = new ConcurrentDictionary<string, RefDataAndCashflowDetails>();
                ConcurrentStack<RefDataAndCashflowDetails> duplicates = new ConcurrentStack<RefDataAndCashflowDetails>();

                Parallel.ForEach(entitesWithoutCashflowRecord, (entity) =>
                {
                    if (!distinctItems.TryAdd(entity.TaxIdentificationNumber, entity))
                    {
                        duplicates.Push(entity);
                    }
                });
                return new ProcessResponseModel { MethodReturnObject = new RefDataDistinctGroupModel { DistinctItems = distinctItems, Duplicates = duplicates } };
            }
            catch (Exception exception)
            {
                return new ProcessResponseModel { HasErrors = true, ErrorMessage = string.Format("Error getting distinct ref data records. Exception: {0} Exceptio Trace: {1}", exception.Message, exception.StackTrace) };
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="shellContext"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        /// <returns>ProcessResponseModel | List{RefDataAndCashflowDetails}</returns>
        public ProcessResponseModel GetCashflowCredentialsAlongWithRefDatadetails(ShellContext shellContext, string batchNumber, int revenueHeadId, int billingId)
        {
            using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
            {
                return workContext.Resolve<ITaxEntityManager<TaxEntity>>().GetRefDataTempJoinerWithTaxIdentification(batchNumber, revenueHeadId, billingId);
            }
        }

        /// <summary>
        /// Group list of <see cref="RefDataAndCashflowDetails"/> into items that have cashflow records and those that havve none
        /// </summary>
        /// <param name="refDataTaxEntityiesJoiner"></param>
        /// <returns>HasCashflowCustomerAndHasNot</returns>
        public HasCashflowCustomerAndHasNot SegmentThoseThatHaveCashflowRecordsAndThoseThatHaveNone(IList<RefDataAndCashflowDetails> refDataTaxEntityiesJoiner)
        {
            ConcurrentStack<RefDataAndCashflowDetails> hasCashFlow = new ConcurrentStack<RefDataAndCashflowDetails>();
            ConcurrentStack<RefDataAndCashflowDetails> hasCashFlowNot = new ConcurrentStack<RefDataAndCashflowDetails>();

            Parallel.ForEach(refDataTaxEntityiesJoiner, (item) =>
            {
                if (item.CashflowCustomerId != 0) { hasCashFlow.Push(item); }
                else { hasCashFlowNot.Push(item); }
            });
            return new HasCashflowCustomerAndHasNot { ItemsWithCashflowDetails = hasCashFlow, ItemsWithoutCashflowDetails = hasCashFlowNot };
        }

        /// <summary>
        /// Get tax entities that belong to this category, with tcategory Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>List<TaxPayerWithDetails></returns>
        public List<TaxPayerWithDetails> GetTaxEntitiesByCategory(int categoryId)
        {
            return _taxPayerRepository.GetListOfTaxPayersWithCategoryId(categoryId);
        }
    }
}