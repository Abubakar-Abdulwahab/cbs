using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System.Collections.Generic;
using System.Web;

namespace Parkway.CBS.OSGOF.Web.Controllers.Handlers.Contracts
{
    public interface ICellSiteHandler : ICommonBaseHandler
    {

        /// <summary>
        /// Process onscreen assessment for OSGOF
        /// </summary>
        /// <param name="cellSites"></param>
        /// <param name="processStage"></param>
        /// <param name="entity"></param>
        /// <param name="cBSUser"></param>
        /// <returns>ProcessingReportVM</returns>
        ProcessingReportVM ProcessOSGOFOnScreenAssessment(ICollection<FileUploadCellSites> cellSites, GenerateInvoiceStepsModel processStage, TaxEntity entity, CBSUser cBSUser);


        /// <summary>
        /// Process file upload assessment for OSGOF
        /// </summary>
        /// <param name="file"></param>
        /// <param name="processStage"></param>
        /// <param name="entity"></param>
        /// <param name="cBSUser"></param>
        /// <returns>ProcessingReportVM</returns>
        ProcessingReportVM ProcessCellSitesFileUpload(HttpPostedFileBase file, GenerateInvoiceStepsModel processStage, TaxEntity entity, CBSUser cBSUser);


        /// <summary>
        /// Process cell sites file upload
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns>APIResponse</returns>
        APIResponse ProcessCellSiteFile(string batchToken);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns></returns>
        APIResponse ValidateCellSitesAgainstStoredRecords(string batchToken);


        /// <summary>
        /// Once the file contents have been read and validated, this method would get the payment details to construct the view
        /// </summary>
        /// <param name="batchToken"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPaymentScheduleDetails(string batchToken);

        /// <summary>
        /// Get the next page for cell site report
        /// </summary>
        /// <param name="batchToken"></param>
        /// <param name="page"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPagedCellSiteData(string batchToken, int page);


        /// <summary>
        /// Get the next paged data for schedule upload for client
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <param name="page"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPagedCellSiteForScheduleUpload(string scheduleRef, int page);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        CellSitesVM GetCellSites(long operatorId, int page, int pageSize);


        /// <summary>
        ///  Get the next paged data for cell sites for operator page display
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        APIResponse GetPagedCellSiteList(long operatorId, int page);


        /// <summary>
        /// Get data from cell sites schedule client side
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>CellSitesStagingReportVM</returns>
        CellSitesStagingReportVM GetStagingDataForSchdedule(string scheduleRef, int take, int skip);

        /// <summary>
        /// Complete cell sites processing for client side cell sites upload
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <returns>CellSitesStagingReportVM</returns>
        CellSitesStagingReportVM CompleteCellSitesProcessingForClientUpload(string scheduleRef);


        /// <summary>
        /// Process cell sites file
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="file"></param>
        /// <returns>CellSitesFileValidationObject</returns>
        CellSitesFileValidationObject CreateCellSites(string payerId, HttpPostedFileBase file, UserPartRecord adminUser, CBSUser loggedInUser);

    }
}
