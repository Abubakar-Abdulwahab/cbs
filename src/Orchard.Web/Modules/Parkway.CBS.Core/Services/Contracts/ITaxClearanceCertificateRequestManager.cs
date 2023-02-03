using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxClearanceCertificateRequestManager<TaxClearanceCertifcateRequest> : IDependency, IBaseManager<TaxClearanceCertifcateRequest>
    {
        /// <summary>
        /// Get TCC request details for TCC request with specified tcc number
        /// </summary>
        /// <param name="tccNumber"></param>
        /// <returns></returns>
        TCCRequestDetailVM GetRequestDetails(string tccNumber);

        /// <summary>
        /// Get TCC request details for certificate generation using the specified application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns>TCCertificateVM</returns>
        TCCertificateVM GetCertificateRequestDetails(string applicationNumber);

        /// <summary>
        /// Get TCC request id using the specified application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns>Int64</returns>
        Int64 GetRequestRequestId(string applicationNumber);
    }
}
