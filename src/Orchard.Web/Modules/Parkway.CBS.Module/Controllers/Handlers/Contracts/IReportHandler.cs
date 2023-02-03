using Orchard;
using System;
using System.Collections.Generic;
using Parkway.CBS.Module.ViewModels;
using Parkway.CBS.Core.Models;
using System.Linq.Expressions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Orchard.Security.Permissions;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IReportHandler : IDependency
    {
        /// <summary>
        /// Get payment report per revenue head
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <param name="slug"></param>
        /// <param name="search"></param>
        /// <param name="direction"></param>
        /// <returns>PerRevenueReportViewModel</returns>
        PerRevenueReportViewModel GetMDAReportPerRevenueHeadCollection(string orderBy, DateTime startDate, DateTime endDate, int count, int skip, string slug, string search, bool direction);

        RevenueHeadInvoicesViewModel GetRevenueHeadInvoicesCollection(string orderBy, DateTime startDate, DateTime endDate, int count, int skip, string slug, int id, string search, bool direction);

        MDAExpectationViewModel GetExpectationForMDACollection(string orderBy, DateTime startDate, DateTime endDate, int count, int skip, string search, bool direction, string slug = "");

        MDAMonthlyPaymentViewModel MDAMonthlyPaymentCollection(string orderBy, DateTime startDate, DateTime endDate, int count, int skip, string search, bool direction);

        MDAMonthlyPaymentPerRevenueViewModel MDAMonthlyPaymentPerRevenueCollection(string orderBy, DateTime startDate, DateTime endDate, int count, int skip, string search, bool direction, string mda);


        RevenueHeadPaymentBreakdownViewModel GetRevenueHeadBreakDownPaymentCollection(string orderBy, DateTime startDate, DateTime endDate, int count, int skip, string slug, int id, string search, bool direction);


        /// <summary>
        /// Get model for invoice assessment report
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns>MDAReportViewModel</returns>
        MDAReportViewModel GetInvoiceAssessmentReport(InvoiceAssessmentSearchParams searchData);


        /// <summary>
        /// Get view for collections
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns>CollectionReportViewModel</returns>
        CollectionReportViewModel GetReportForCollection(CollectionSearchParams searchData);



        IEnumerable<MDA> MDAs(MDAFilter filter = MDAFilter.All);

        IEnumerable<RevenueHeadDropDownListViewModel> GetRevenueHeads(string mdaSlug);

        /// <summary>
        /// Get the list of revenue heads that have been assigned to this user
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="accessType"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetRevenueHeads(string mdaId, AccessType accessType);


        DemandNoticeViewModel GetDemandNoticeReport(DateTime startDate, DateTime endDate, string mda, string revenueheadId, PaymentOptions options, int take, int skip);

        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void CheckForPermission(Permission permission);

        
        /// <summary>
        /// Get tax profile report
        /// </summary>
        /// <param name="model"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>TaxProfilesReportVM</returns>
        TaxProfilesReportVM GetTaxProfilesReport(TaxProfilesReportVM model, int page, int pageSize);


        /// <summary>
        /// Get tax payer details
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns>TaxPayerDetailsViewModel</returns>
        TaxPayerDetailsViewModel GetTaxPayer(string payerId);


        /// <summary>
        /// Get the list of tax categories that are active
        /// </summary>
        /// <returns>IEnumerable{TaxEntityCategoryVM}</returns>
        IEnumerable<TaxEntityCategoryVM> GetTaxCategories();



        bool UpdateTaxPayer(TaxPayerDetailsViewModel model);



        bool CheckIfTaxPayerCodeExist(string taxPayerCode, string taxPayerId);

    }
}
