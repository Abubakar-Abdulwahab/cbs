using Orchard.Environment;
using Orchard.Environment.Configuration;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Models;
using Orchard.Environment.ShellBuilders;
using System.Data;
using System.Diagnostics;
using Orchard.Logging;
using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Autofac;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Concurrent;
using Parkway.Cashflow.Ng.Models;
using System.Threading.Tasks;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.ReferenceData.DataSource.Contracts;
using Parkway.CBS.ReferenceData;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreInvoiceServiceRefDataExtension : ICoreInvoiceServiceRefDataExtension
    {
        private readonly IEnumerable<IReferenceDataSource> _refDataSources;
        private readonly IShellSettingsManager _shellSettingsManager;
        private readonly IOrchardHost _orchardHost;
        private readonly ICoreTaxPayerService _taxPayerService;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }
        private readonly IInvoicingService _invoicingService;


        public CoreInvoiceServiceRefDataExtension(IOrchardServices orchardServices, IShellSettingsManager shellSettingsManager, IOrchardHost orchardHost, IEnumerable<IReferenceDataSource> refDataSources, ICoreTaxPayerService taxPayerService, IInvoicingService invoicingService)
        {
            _orchardServices = orchardServices;
            _shellSettingsManager = shellSettingsManager;
            _orchardHost = orchardHost;
            _refDataSources = refDataSources;
            _taxPayerService = taxPayerService;
            Logger = NullLogger.Instance;
            _invoicingService = invoicingService;
        }


        /// <summary>
        /// Store reference data
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="tenant"></param>
        public void StoreReferenceData(ShellContext shellContext, List<RefDataTemp> entities, ExpertSystemSettings tenant)
        {
            string tableName = "Parkway_CBS_Core_RefDataTemp";
            SaveRefDataInTempTable(entities, shellContext, tableName);
        }


        /// <summary>
        /// Get shell context for thsi operation
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns>ShellContext</returns>
        public ShellContext GetShellContext(string siteName)
        {
            var tenantShellSettings = _shellSettingsManager.LoadSettings().Where(settings => settings.Name == siteName).Single();
            return _orchardHost.GetShellContext(tenantShellSettings);
        }


        /// <summary>
        /// Save ref data 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="shellContext"></param>
        /// <param name="tableName"></param>
        private void SaveRefDataInTempTable(List<RefDataTemp> entities, ShellContext shellContext, string tableName)
        {
            //save entities into temp table
            int chunkSize = 500000;
            var dataSize = entities.Count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;
            int counter = 0;
            var startTime = Stopwatch.StartNew();

            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable(tableName);
                    dataTable.Columns.Add(new DataColumn("Recipient"));
                    dataTable.Columns.Add(new DataColumn("Address"));
                    dataTable.Columns.Add(new DataColumn("TaxEntityCategoryId", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("BillingModelId", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("RevenueHeadId", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("TaxIdentificationNumber"));
                    dataTable.Columns.Add(new DataColumn("UniqueIdentifier"));
                    dataTable.Columns.Add(new DataColumn("Email"));
                    dataTable.Columns.Add(new DataColumn("BatchNumber"));
                    dataTable.Columns.Add(new DataColumn("Status", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("AdditionalDetails"));
                    dataTable.Columns.Add(new DataColumn("StatusDetail"));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    int index = 0;
                    entities.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["Recipient"] = x.Recipient;
                        row["Address"] = x.Address;
                        row["TaxEntityCategoryId"] = x.TaxEntityCategoryId;
                        row["BillingModelId"] = x.BillingModelId;
                        row["RevenueHeadId"] = x.RevenueHeadId;
                        row["TaxIdentificationNumber"] = x.TaxIdentificationNumber;
                        row["UniqueIdentifier"] = x.TaxIdentificationNumber + "#" + x.BatchNumber + "#" + index++;
                        row["AdditionalDetails"] = x.AdditionalDetails;
                        row["Email"] = x.Email;
                        row["BatchNumber"] = x.BatchNumber;
                        row["Status"] = x.Status;
                        row["StatusDetail"] = x.StatusDetail;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });

                    Logger.Error(string.Format("Ref Data insertion has started Size: {0} Skip: {1} Time: {2}", dataSize, skip, startTime.Elapsed.ToString()));
                    // Creating a new work context to run our code. Resolve() needs using Autofac;
                    // You can resolve services from the tenant that you would normally inject through the constructor.  
                    using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                    {
                        workContext.Resolve<IRefDataTempManager<RefDataTemp>>().SaveBundle(dataTable, tableName);
                    }
                    skip += chunkSize;
                    stopper++;
                    Logger.Error("Counter " + ++counter);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                //TODO
            }
            startTime.Stop();
            Logger.Error(string.Format("RUNTIME - {0} SIZE: {1}", startTime.Elapsed, dataSize));
        }


        /// <summary>
        /// Get a collection that has ref data with their corresponding cashflow customer credentials and ref data items
        /// </summary>
        /// <param name="shellContext"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        /// <returns>ProcessResponseModel | List{RefDataAndCashflowDetails}</returns>
        public ProcessResponseModel GetRefDataWithTheirCorrespondingCashflowCredentials(ShellContext shellContext, string batchNumber, int revenueHeadId, int billingId)
        {
            return _taxPayerService.GetCashflowCredentialsAlongWithRefDatadetails(shellContext, batchNumber, revenueHeadId, billingId);
        }


        /// <summary>
        /// Update ref data table records
        /// </summary>
        /// <param name="shellContext"></param>
        /// <param name="columnAndValue"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        public void UpdateRefDataTemp(ShellContext shellContext, Dictionary<string, string> columnAndValue, string batchNumber, int revenueHeadId, int billingId)
        {
            using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
            {
                workContext.Resolve<IRefDataTempManager<RefDataTemp>>().UpdateRefDataTemp(columnAndValue, batchNumber, revenueHeadId, billingId);
            }
        }


        /// <summary>
        /// Get the group of entites that are disticnt and also their duplicates
        /// </summary>
        /// <param name="entitesWithoutCashflowRecord"></param>
        /// <returns>ProcessResponseModel | if no errors <see cref="RefDataDistinctGroupModel"/>RefDataDistinctGroupModel</returns>
        public ProcessResponseModel SegmentEntitiesWithoutCashflowRecordsIntoUniqueItemAndDuplicates(ConcurrentStack<RefDataAndCashflowDetails> entitesWithoutCashflowRecord)
        {
            return _taxPayerService.GetDistinctRefDataRecordsItemsAndDuplicates(entitesWithoutCashflowRecord);
        }


        /// <summary>
        /// Get a collection of create cashflow customer and invoice models
        /// </summary>
        /// <param name="itemsWithCashflowDetails"></param>
        /// <param name="distinctRefDataEntities"></param>
        /// <param name="revenueHead"></param>
        /// <param name="mda"></param>
        /// <param name="billing"></param>
        /// <returns>ConcurrentStack{CashFlowCreateCustomerAndInvoice}</returns>
        public ConcurrentStack<CashFlowRequestModelAndRefDataItem> GetCashflowCustomerAndInvoiceModel(ExpertSystemSettings tenant, RevenueHead revenueHead, MDA mda, BillingModel billing, CreateInvoiceHelper invoiceHelper, ConcurrentStack<RefDataAndCashflowDetails> itemsWithCashflowDetails, ConcurrentDictionary<string, RefDataAndCashflowDetails> distinctRefDataEntities)
        {
            ConcurrentStack<CashFlowRequestModelAndRefDataItem> customerAndInvoiceCollection = new ConcurrentStack<CashFlowRequestModelAndRefDataItem>();
            //for records with cash flow customer
            if (itemsWithCashflowDetails.Count < 5000)
            {
                foreach (var item in itemsWithCashflowDetails)
                {
                    customerAndInvoiceCollection.Push(new CashFlowRequestModelAndRefDataItem
                    {
                        RefDataAndCashflowDetails = item,
                        CashFlowRequestModel = new CashFlowCreateCustomerAndInvoice
                        {
                            CreateCustomer = CreateCashflowCustomer(tenant, item),
                            CreateInvoice = CreateCashflowCustomerInvoice(item, invoiceHelper)
                        }
                    });
                }
            }
            else
            {
                Parallel.ForEach(itemsWithCashflowDetails, (item) =>
                {
                    customerAndInvoiceCollection.Push(new CashFlowRequestModelAndRefDataItem
                    {
                        RefDataAndCashflowDetails = item,
                        CashFlowRequestModel = new CashFlowCreateCustomerAndInvoice
                        {
                            CreateCustomer = CreateCashflowCustomer(tenant, item),
                            CreateInvoice = CreateCashflowCustomerInvoice(item, invoiceHelper)
                        }
                    });
                });
            }
            //for records without cash flow customer
            if (distinctRefDataEntities.Count < 5000)
            {
                foreach (var item in distinctRefDataEntities)
                {
                    customerAndInvoiceCollection.Push(new CashFlowRequestModelAndRefDataItem
                    {
                        RefDataAndCashflowDetails = item.Value,
                        CashFlowRequestModel = new CashFlowCreateCustomerAndInvoice
                        {
                            CreateCustomer = CreateCashflowCustomer(tenant, item.Value),
                            CreateInvoice = CreateCashflowCustomerInvoice(item.Value, invoiceHelper)
                        }
                    });
                }
            }
            else
            {
                Parallel.ForEach(distinctRefDataEntities, (item) =>
                {
                    customerAndInvoiceCollection.Push(new CashFlowRequestModelAndRefDataItem
                    {
                        RefDataAndCashflowDetails = item.Value,
                        CashFlowRequestModel = new CashFlowCreateCustomerAndInvoice
                        {
                            CreateCustomer = CreateCashflowCustomer(tenant, item.Value),
                            CreateInvoice = CreateCashflowCustomerInvoice(item.Value, invoiceHelper)
                        }
                    });
                });
            }
            return customerAndInvoiceCollection;
        }


        /// <summary>
        /// Generate model for cashflow invoice
        /// </summary>
        /// <param name="item"></param>
        /// <param name="invoiceHelper"></param>
        /// <returns>CashFlowCreateInvoice</returns>
        private CashFlowCreateInvoice CreateCashflowCustomerInvoice(RefDataAndCashflowDetails item, CreateInvoiceHelper invoiceHelper)
        {
            return new CashFlowCreateInvoice
            {
                ContactID = item.CashflowPrimaryContactId,
                CustomerId = item.CashflowCustomerId,
                Discount = invoiceHelper.DiscountModel != null ? invoiceHelper.DiscountModel.Discount : 0m,
                DiscountType = invoiceHelper.DiscountModel != null ? invoiceHelper.DiscountModel.BillingDiscountType.ToString() : "",
                DueDate = invoiceHelper.DueDate,
                FootNote = invoiceHelper.FootNotes,
                InvoiceDate = invoiceHelper.InvoiceDate,
                Items = new List<CashFlowCreateInvoice.CashFlowProductModel>
                {
                    new CashFlowCreateInvoice.CashFlowProductModel
                    {
                        Pos = 1,
                        Price = item.Amount != 0?item.Amount:invoiceHelper.Amount,
                        ProductId = invoiceHelper.Items.ElementAt(0).ProductId,
                        ProductName = invoiceHelper.Items.ElementAt(0).ProductName,
                        Qty = 1,
                    }
                },
                Title = invoiceHelper.Title,
                Type = invoiceHelper.Type,
            };
        }


        /// <summary>
        /// get model for creating customer on cashflow
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="item"></param>
        /// <returns>CashFlowCreateCustomer</returns>
        private CashFlowCreateCustomer CreateCashflowCustomer(ExpertSystemSettings tenant, RefDataAndCashflowDetails item)
        {
            return new CashFlowCreateCustomer
            {
                Address = item.Address,
                CountryID = tenant.TenantCBSSettings.CountryId,
                CustomerId = item.CashflowCustomerId,
                Identifier = item.TaxIdentificationNumber,
                Name = item.Recipient,
                StateID = tenant.TenantCBSSettings.StateId,
                Type = item.TaxEntityCategoryId == 0 ? Cashflow.Ng.Models.Enums.CashFlowCustomerType.Individual : Cashflow.Ng.Models.Enums.CashFlowCustomerType.Business,
                PryContact = new CashFlowCreateCustomer.Contact
                {
                    Name = item.Recipient,
                    Email = item.Email,
                }
            };
        }


        public IEnumerable<CashflowCreateCustomerAndGenerateInvoiceResponseModel> CreateCustomerAndGenerateInvoiceOnCashflow(string smeKey, ConcurrentStack<CashFlowRequestModelAndRefDataItem> customerAndInvoices)
        {
            return GetUsersAsync(smeKey, customerAndInvoices).Result;
        }

        public async Task<IEnumerable<CashflowCreateCustomerAndGenerateInvoiceResponseModel>> GetUsersAsync(string key, ConcurrentStack<CashFlowRequestModelAndRefDataItem> customerAndInvoices)
        {
            ConcurrentStack<Task<CashflowCreateCustomerAndGenerateInvoiceResponseModel>> listOfProcessResults = new ConcurrentStack<Task<CashflowCreateCustomerAndGenerateInvoiceResponseModel>>();

            Parallel.ForEach(customerAndInvoices, (customerAndInvoice) =>
            {
                listOfProcessResults.Push(RunProcess(key, customerAndInvoice));
            });
            return await Task.WhenAll(listOfProcessResults);
        }

        private async Task<CashflowCreateCustomerAndGenerateInvoiceResponseModel> RunProcess(string SMEKey, CashFlowRequestModelAndRefDataItem item)
        {
            throw new Exception();
            //try
            //{
            //    var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", SMEKey } });
            //    var invoiceService = _invoicingService.InvoiceService(context);
            //    return new CashflowCreateCustomerAndGenerateInvoiceResponseModel { RequestModel = item.CashFlowRequestModel, RefDataItem = item.RefDataAndCashflowDetails, ResponseModel = await invoiceService.CreateCustomerAndInvoiceAsync(item.CashFlowRequestModel) };
            //}
            //catch (Exception exception)
            //{
            //    return new CashflowCreateCustomerAndGenerateInvoiceResponseModel { RequestModel = item.CashFlowRequestModel, RefDataItem = item.RefDataAndCashflowDetails, ResponseModel = new ResponseModel { HasErrors = true, Response = new ErrorResponse { ErrorCode = "", ErrorMessage = string.Format("Exception on Central Billing while running process to create customer and generate invoice on cashflow. Exception: {0} StackTrace: {1}", exception.Message, exception.StackTrace) } } };
            //}
        }


        /// <summary>
        /// Seperate the successful and unsuccful cashflow responses
        /// </summary>
        /// <param name="cashflowCreateCustomerAndInvoiceGenerationResponse"></param>
        /// <returns>SuccessfulAndUnSuccessfulCashflowResponses</returns>
        public SuccessfulAndUnSuccessfulCashflowResponses SegmentCashflowResponseIntoSuccessfulAndUnsuccessfulResponses(IEnumerable<CashflowCreateCustomerAndGenerateInvoiceResponseModel> cashflowCreateCustomerAndInvoiceGenerationResponse)
        {
            ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel> successfulCashFlowResponses = new ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel>();

            ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel> unSuccessfulCashFlowResponses = new ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel>();

            Parallel.ForEach(cashflowCreateCustomerAndInvoiceGenerationResponse, (response) =>
            {
                //if (response.ResponseModel.HasErrors) { unSuccessfulCashFlowResponses.Push(response); }
                //else { successfulCashFlowResponses.Push(response); }
            });

            return new SuccessfulAndUnSuccessfulCashflowResponses { SuccessfulCashFlowResponses = successfulCashFlowResponses, UnSuccessfulCashFlowResponses = unSuccessfulCashFlowResponses };
        }

        public void SaveSuccessfulInvoiceResponses(ShellContext shellContext, MDA mda, RevenueHead revenueHead, ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel> successfulCashFlowResponses, string tableName, DateTime dueDate)
        {
            //save entities into temp table
            int chunkSize = 500000;
            var dataSize = successfulCashFlowResponses.Count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;
            int counter = 0;
            var startTime = Stopwatch.StartNew();

            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable(tableName);
                    dataTable.Columns.Add(new DataColumn("InvoiceNumber"));
                    dataTable.Columns.Add(new DataColumn("MDA_Id"));
                    dataTable.Columns.Add(new DataColumn("RevenueHead_Id"));
                    dataTable.Columns.Add(new DataColumn("InvoiceURL"));
                    dataTable.Columns.Add(new DataColumn("Status", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("Amount", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("AmountDue", typeof(decimal)));
                    dataTable.Columns.Add(new DataColumn("DueDate", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("TaxPayerCategory_Id"));
                    dataTable.Columns.Add(new DataColumn("TaxIdentificationNumber"));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    successfulCashFlowResponses.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        CashFlowCreateCustomerAndInvoiceResponse cashflowResponse = new CashFlowCreateCustomerAndInvoiceResponse();//x.ResponseModel.Response;
                        var row = dataTable.NewRow();
                        row["InvoiceNumber"] = cashflowResponse.Invoice.Number;
                        row["MDA_Id"] = mda.Id;
                        row["RevenueHead_Id"] = revenueHead.Id;
                        row["InvoiceURL"] = cashflowResponse.Invoice.PdfUrl;
                        row["Status"] = (int)InvoiceStatus.Unpaid;
                        row["Amount"] = cashflowResponse.Invoice.AmountDue;
                        row["AmountDue"] = cashflowResponse.Invoice.AmountDue;
                        row["DueDate"] = dueDate;
                        row["TaxPayerCategory_Id"] = x.RefDataItem.TaxEntityCategoryId;
                        row["TaxIdentificationNumber"] = x.RefDataItem.TaxIdentificationNumber;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });
                    Logger.Error(string.Format("invoice Data insertion has started Size: {0} Skip: {1} Time: {2}", dataSize, skip, startTime.Elapsed.ToString()));
                    // Creating a new work context to run our code. Resolve() needs using Autofac;
                    // You can resolve services from the tenant that you would normally inject through the constructor.  
                    using (var workContext = shellContext.LifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope())
                    {
                        workContext.Resolve<IInvoiceManager<Invoice>>().SaveBundle(dataTable, tableName);
                    }
                    skip += chunkSize;
                    stopper++;
                    Logger.Error("Counter " + ++counter);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                //TODO
            }
            startTime.Stop();
            Logger.Error(string.Format("RUNTIME - {0} SIZE: {1}", startTime.Elapsed, dataSize));
        }

        public void UpdateRefDataTempOfUnsuccessfulResponses(ShellContext shellContext, string batchNumber, int revenueHeadId, int billingId, ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel> unSuccessfulCashFlowResponses)
        {
            throw new NotImplementedException();
        }
    }
}