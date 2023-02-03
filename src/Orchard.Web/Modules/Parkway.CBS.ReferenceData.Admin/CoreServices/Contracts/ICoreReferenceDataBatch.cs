using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.ClientFileServices.Implementations.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee.ReferenceDataImplementation;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.CoreServices.Contracts
{
    public interface ICoreReferenceDataBatch : IDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<ReferenceDataBatch> GetReferenceDataRecords(int skip, int take, ReferenceDataBatchSearchParams searchParams);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="adminUser"></param>
        /// <param name="filePath"></param>
        /// <returns>ValidateFileResponseVM</returns>
        ValidateFileResponseVM SaveFile(HttpPostedFileBase file, UserPartRecord adminUser, ValidateFileModel model);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<ReferenceDataBatchReportStats> GetAggregateReferenceDataRecords(ReferenceDataBatchSearchParams searchParams);


        /// <summary>
        /// Get a batch details
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>string</returns>
        string GetReferenceDataBatchRef(int Id);

        /// <summary>
        /// Get XMLFile config upload Interfaces
        /// </summary>
        /// <param name="siteName"></param>
        /// <returns></returns>
        IEnumerable<AdaptersVM> GetUploadInterfaces(string stateName);

        /// <summary>
        /// Get Reference Data details using BatchId
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns>ReferenceDataBatch</returns>
        ReferenceDataBatch GetReferenceDataBatch(string batchRef);
    }
}
