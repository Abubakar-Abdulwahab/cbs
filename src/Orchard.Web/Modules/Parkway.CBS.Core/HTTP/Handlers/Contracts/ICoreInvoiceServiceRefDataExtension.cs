using Orchard;
using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Orchard.Environment.ShellBuilders;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Concurrent;
using Parkway.CBS.ReferenceData;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreInvoiceServiceRefDataExtension : IDependency
    {
        /// <summary>
        /// Get shell context for thsi operation
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns>ShellContext</returns>
        ShellContext GetShellContext(string siteName);


        /// <summary>
        /// Store reference data
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="tenant"></param>
        void StoreReferenceData(ShellContext shellContext, List<RefDataTemp> entities, ExpertSystemSettings tenant);


        /// <summary>
        /// Get all ref data and their corresponding tax entity which include cashflow customer details if they exists
        /// </summary>
        /// <param name="shellContext"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        /// <returns>ProcessResponseModel | if has no errors in ProcessResponseModel, get IList{RefDataAndCashflowDetails} from method return object</returns>
        ProcessResponseModel GetRefDataWithTheirCorrespondingCashflowCredentials(ShellContext shellContext, string batchNumber, int revenueHeadId, int billingId);


        /// <summary>
        /// Update ref data table records
        /// </summary>
        /// <param name="shellContext"></param>
        /// <param name="columnAndValue"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        void UpdateRefDataTemp(ShellContext shellContext, Dictionary<string, string> columnAndValue, string batchNumber, int revenueHeadId, int billingId);


        /// <summary>
        /// Get the group of entites that are disticnt and also their duplicates
        /// </summary>
        /// <param name="entitesWithoutCashflowRecord"></param>
        /// <returns>ProcessResponseModel | if no errors <see cref="RefDataDistinctGroupModel"/>RefDataDistinctGroupModel</returns>
        ProcessResponseModel SegmentEntitiesWithoutCashflowRecordsIntoUniqueItemAndDuplicates(ConcurrentStack<RefDataAndCashflowDetails> itemsWithoutCashflowDetails);


        /// <summary>
        /// Get a collection of create cashflow customer and invoice models
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="revenueHead"></param>
        /// <param name="mda"></param>
        /// <param name="billing"></param>
        /// <param name="invoiceHelper"></param>
        /// <param name="itemsWithCashflowDetails"></param>
        /// <param name="distinctRefDataEntities"></param>
        /// <returns>ConcurrentStack{CashFlowCreateCustomerAndInvoice}</returns>
        ConcurrentStack<CashFlowRequestModelAndRefDataItem> GetCashflowCustomerAndInvoiceModel(ExpertSystemSettings tenant, RevenueHead revenueHead, MDA mda, BillingModel billing, CreateInvoiceHelper invoiceHelper, ConcurrentStack<RefDataAndCashflowDetails> itemsWithCashflowDetails, ConcurrentDictionary<string, RefDataAndCashflowDetails> distinctRefDataEntities);


        /// <summary>
        /// Create customer and generate invoices on cashflow
        /// </summary>
        /// <param name="smeKey">Cashflow company key code</param>
        /// <param name="customerAndInvoices"></param>
        /// <returns>IEnumerable{CashflowCreateCustomerAndGenerateInvoiceResponseModel}</returns>
        IEnumerable<CashflowCreateCustomerAndGenerateInvoiceResponseModel> CreateCustomerAndGenerateInvoiceOnCashflow(string smeKey, ConcurrentStack<CashFlowRequestModelAndRefDataItem> customerAndInvoices);


        /// <summary>
        /// Seperate the successful and unsuccful cashflow responses
        /// </summary>
        /// <param name="cashflowCreateCustomerAndInvoiceGenerationResponse"></param>
        /// <returns>SuccessfulAndUnSuccessfulCashflowResponses</returns>
        SuccessfulAndUnSuccessfulCashflowResponses SegmentCashflowResponseIntoSuccessfulAndUnsuccessfulResponses(IEnumerable<CashflowCreateCustomerAndGenerateInvoiceResponseModel> cashflowCreateCustomerAndInvoiceGenerationResponse);


        /// <summary>
        /// Save successful response
        /// </summary>
        /// <param name="successfulCashFlowResponses"></param>
        void SaveSuccessfulInvoiceResponses(ShellContext shellContext, MDA mda, RevenueHead revenueHead, ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel> successfulCashFlowResponses, string tableName, DateTime dueDate);


        /// <summary>
        /// Update unsuccessful response to ref data
        /// </summary>
        /// <param name="shellContext"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        /// <param name="unSuccessfulCashFlowResponses"></param>
        void UpdateRefDataTempOfUnsuccessfulResponses(ShellContext shellContext, string batchNumber, int revenueHeadId, int billingId, ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel> unSuccessfulCashFlowResponses);
    }
}
