using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Text.RegularExpressions;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class TCCValidationHandler : ITCCValidationHandler
    {
        private readonly Lazy<ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest>> _tccRequestManager;

        public TCCValidationHandler( Lazy<ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest>> tccRequestManager)
        {
            _tccRequestManager = tccRequestManager;
        }

        /// <summary>
        /// Validate application number format
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns></returns>
        public bool ValidateApplicationNumberFormat(string applicationNumber)
        {
            try
            {
                string pattern = @"\b(TCC_)(\d{10,})\b";
                if (Regex.IsMatch(applicationNumber, pattern))
                {
                    return true;
                }
                else { return false; }
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Get request details for TCC with specified application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns></returns>
        public TCCRequestDetailVM GetRequestDetail(string applicationNumber)
        {
            try
            {
                return _tccRequestManager.Value.GetRequestDetails(applicationNumber);
            }
            catch (Exception) { throw; }
            
        }
    }
}