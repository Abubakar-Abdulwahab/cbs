using Orchard;
using Orchard.Security.Permissions;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.FileUpload;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.Controllers.Handlers.Contracts
{
    public interface IDataEnumerationHandler : IDependency
    {
        /// <summary>
        /// Upload the enumeration data file to the specified folder
        /// </summary>
        /// <param name="file"></param>
        /// <param name="adminUser"></param>
        /// <returns>ValidateFileResponseVM</returns>
        ValidateFileResponseVM ProcessEnumerationDataFile(HttpPostedFileBase file, UserPartRecord adminUser, ValidateFileModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        ReferenceDataBatchVM GetCollectionReport(int skip, int take, ReferenceDataBatchSearchParams searchParams);

        /// <summary>
        /// Get Reference Data BatchRef using BatchId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string</returns>
        string GetReferenceDataBatchRef(int id);

        /// <summary>
        /// Get NAGIS Data BatchRef using BatchId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetNAGISDataBatchRef(int Id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        NAGISDataBatchVM GetCollectionReport();

        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void CheckForPermission(Permission permission);


        /// <summary>
        /// Get the list of LGAs for a particular state
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        ValidateFileModel GetLGAs(int stateId);

        /// <summary>
        /// Get the list of LGAs for this tenant
        /// </summary>
        /// <returns>ValidateFileModel</returns>
        ValidateFileModel GetLGAsAndAdapters();

        /// <summary>
        /// Get Reference Data details using BatchId
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns>ReferenceDataBatch</returns>
        ReferenceDataBatch GetReferenceDataBatch(string batchRef);

        /// <summary>
        /// Get collection of NAGIS Old Invoice Migration records
        /// </summary>
        /// <param name="nagisDataBatchId"></param>
        /// <returns></returns>
        NAGISInvoiceSummaryVM GetNAGISInvoiceSummaryCollection(long nagisDataBatchId);

    }
}
