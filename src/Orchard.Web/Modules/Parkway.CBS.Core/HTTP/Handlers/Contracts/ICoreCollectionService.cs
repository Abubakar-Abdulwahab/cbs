using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Payee.PayeeAdapters;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Payee;
using System;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreCollectionService : IDependency
    {

        /// <summary>
        /// Get view for collections
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns>CollectionReportViewModel</returns>
        CollectionReportViewModel GetReportForCollection(CollectionSearchParams searchParams, bool populateDropDowns = true);


        /// <summary>
        /// Get record details for collection report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>CollectionReportViewModel</returns>
        CollectionReportViewModel GetCollectionReport(CollectionSearchParams searchParams);



        /// <summary>
        /// Get the assigned form fields
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        IEnumerable<FormControlViewModel> GetRevenueHeadFormFields(int revenueHeadId, int categoryId);


        /// <summary>
        /// Get revenue head details by revenue head Id
        /// </summary>
        /// <param name="id">Id of the revenue head</param>
        /// <returns>RevenueHeadDetails</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        RevenueHeadDetails GetRevenueHeadDetails(int id);


        /// <summary>
        /// Get tax category
        /// </summary>
        /// <param name="catId"></param>
        /// <returns>TaxEntityCategory</returns>
        /// <exception cref="NoCategoryFoundException"></exception>
        TaxEntityCategory GetTaxEntityCategory(int catId);


        /// <summary>
        /// Generate invoice for tax payer
        /// <para>returns a list of errors if any occured while creating one</para>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        InvoiceGeneratedResponseExtn GenerateInvoice(CreateInvoiceModel model, ref List<ErrorModel> errors);


        /// <summary>
        /// Get the billing model for this revenue head
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>BillingModel</returns>
        BillingModel GetRevenueHeadBillingModel(int revenueHeadId);


        /// <summary>
        /// Get batch record
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns>DirectAssessmentBatchRecord | null</returns>
        DirectAssessmentBatchRecord GetBatchRecord(long batchRecordId);


        /// <summary>
        /// Get report for batch record
        /// </summary>
        /// <param name="record"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns>DirectAssessmentReportVM</returns>
        DirectAssessmentReportVM GetPayeAsessmentReport(DirectAssessmentBatchRecord record, int take, int skip, TaxEntity entity = null);


        /// <summary>
        /// Get form details from the DB
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        List<FormControlViewModel> GetFormDetailsFromDB(int revenueHeadId, int categoryId);


        /// <summary>
        /// Get invoice URL
        /// </summary>
        /// <param name="bin"></param>
        /// <returns>string</returns>
        /// <exception cref="NoInvoicesMatchingTheParametersFoundException"></exception>
        string GetInvoiceURL(string bin);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="receiptNumber"></param>
        /// <param name="options"></param>
        /// <param name="datefilter"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        ReceiptObj GetReciepts(string phoneNumber, string receiptNumber, ReceiptStatus status, DateTime startDate, DateTime endDate, int skip, int take, bool queryForCount = false);


        /// <summary>
        /// get the list of MDAs
        /// </summary>
        /// <returns>IEnumerable{MDA}</returns>
        IEnumerable<MDA> GetMDAs();        


        /// <summary>
        /// Get invoice VM for make payment view
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        InvoiceGeneratedResponseExtn GetInvoiceGeneratedResponseObjectForPaymentView(string invoiceNumber);

    }
}
