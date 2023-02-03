using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.Payee;
using Parkway.CBS.Payee.PayeeAdapters.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreDirectAssessmentBatchRecord : IDependency
    {
        /// <summary>
        /// Save a direct assessment batch record
        /// </summary>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="userProfile"></param>
        /// <param name="amount"></param>
        /// <param name="adapter"></param>
        /// <param name="siteName"></param>
        /// <param name="type"></param>
        /// <returns>DirectAssessmentBatchRecord</returns>
        DirectAssessmentBatchRecord SaveDirectAssessmentRecord(RevenueHeadDetails revenueHeadDetails, UserDetailsModel userProfile, decimal amount, string siteName, PayeAssessmentType type);

        /// <summary>
        /// Save the posted file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        void SaveFile(HttpPostedFileBase file, string path);


        /// <summary>
        /// Validate uploaded file
        /// </summary>
        /// <param name="file"></param>
        /// <returns>List{ErrorModel}</returns>
        List<ErrorModel> ValidateFile(HttpPostedFileBase file);


        /// <summary>
        /// Get the assessment interface for this adapter value
        /// </summary>
        /// <param name="adapterValue"></param>
        /// <returns>AssessmentInterface</returns>
        AssessmentInterface GetDirectAssessmentAdapter(string adapterValue);


        /// <summary>
        /// Get the adapter value, read the file upload.
        /// <para>HeaderValidateObject is returned in the return object, check that the error value. 
        /// If true, there has been a mismatch in the header values, the error message is included in the HeaderValidateObject ErrorMessage prop.
        /// If HeaderValidateObject error prop is false the payes are returned
        /// </para>
        /// </summary>
        /// <param name="adapterValue">Computation Adapter</param>
        /// <param name="adapter">Computation Adapter</param>
        /// <param name="filePath">File path to the excel file</param>
        /// <returns>GetPayeResponse</returns>
        GetPayeResponse GetPayes(AssessmentInterface adapter, string filePath);


        /// <summary>
        /// Get computation implementation
        /// </summary>
        /// <param name="adapter">AssessmentInterface</param>
        /// <returns>IPayeeAdapter</returns>
        IPayeeAdapter GetAdapterImplementation(AssessmentInterface adapter);


        /// <summary>
        /// Get the direct payee assessment for the given period
        /// </summary>
        /// <param name="payePeriod"></param>
        DirectAssessmentBatchRecord GetPayeAssessmentByMonthAndYear(string agencyCode, PayeAssessmentType type, DateTime payePeriod);

        /// <summary>
        /// Save unreconcile paye payments
        /// </summary>
        /// <param name="unreconciledPayePayments"></param>
        void SaveUnreconciledPayePayments(UnreconciledPayePayments unreconciledPayePayments);
    }
}
