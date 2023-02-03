using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.TIN.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.TIN.Controllers.Handlers.Contracts
{
    public interface ITINHandler : IDependency
    {
        /// <summary>
        /// Try Register TIN Applicant
        /// </summary>
        /// <param name="formData"></param>
        bool TryRegisterTIN(TINFormViewModel formData);

        /// <summary>
        /// Generate a dummy TIN 
        /// </summary>
        /// <returns></returns>
        long GetLastGeneratedTin();

        /// <summary>
        /// returns list of applicanst for TIN Form
        /// </summary>
        /// <returns></returns>
        IEnumerable<TINApplicantReportModel> GetTINApplicantReport();

        /// <summary>
        /// returns list of applicanst for TIN Form based on search criteria
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        IEnumerable<TINApplicantReportModel> TINApplicantReportSearch(TINSearchParameters search);
        bool UpdateTIN(long tINId, string TINValue);
    }
}
