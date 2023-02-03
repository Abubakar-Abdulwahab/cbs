using Orchard;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.ViewModels;

namespace Parkway.CBS.OSGOF.Web.CoreServices.Contracts
{
    public interface ICoreOSGOFService : IDependency
    {
        /// <summary>
        /// Save OSGOF Batch Record
        /// </summary>
        /// <param name="batch">CellSiteClientPaymentBatch</param>
        void SaveCellSiteClientPaymentBatch(CellSiteClientPaymentBatch batch);


        TaxEntityCategory GetTaxEntityCategory(int categoryId);


        /// <summary>
        /// Get some details about this revenue head
        /// <para>Gets you the revenue head, mda, and billing</para>
        /// </summary>
        /// <param name="revenueHeadId">Revenue head Id</param>
        /// <returns>RevenueHeadDetails</returns>
        RevenueHeadDetails GetRevenueHeadDetails(int revenueHeadId);


        /// <summary>
        /// Save the cellsites for onscreen input
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="response"></param>
        /// <param name="entity"></param>
        void SaveCellSitesForOnScreenInput(CellSiteClientPaymentBatch batch, CellSitesBreakDown cellSitesObj, TaxEntity entity);


        /// <summary>
        /// Get batch record
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns>CellSiteClientPaymentBatch</returns>
        CellSiteClientPaymentBatch GetBatchRecord(long batchRecordId);


        /// <summary>
        /// Get the cell site report
        /// </summary>
        /// <param name="record"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <param name="entity"></param>
        /// <returns>CellSiteReportVM</returns>
        CellSiteReportVM GetCellSitesReport(CellSiteClientPaymentBatch record, int take, int skip, TaxEntity entity, bool getStats = false);


        /// <summary>
        /// Clear nhibernate session from memory
        /// </summary>
        void ClearSession();


        /// <summary>
        /// Process cell site file upload 
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns>APIResponse</returns>
        APIResponse ProcessCellSiteFileUpload(FileProcessModel objValue);


        /// <summary>
        /// Do a cell sites comparison between the cell sites added by the user against the ones on the system
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns>APIResponse</returns>
        APIResponse DoCellSitesComparison(FileProcessModel objValue);


        /// <summary>
        /// Get list of cell sites
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name=""></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>List{CellSiteReturnModelVM}</returns>
        List<CellSiteReturnModelVM> GetCellSites(FileProcessModel objValue, int take, int skip);


        APIResponse GetScheduleDetails(FileProcessModel objValue);

        /// <summary>
        /// Get the total amount due on the given schedule
        /// </summary>
        /// <param name="record"></param>
        /// <returns>decimal</returns>
        decimal GetTotalAmountForBatch(CellSiteClientPaymentBatch record);
    }
}
