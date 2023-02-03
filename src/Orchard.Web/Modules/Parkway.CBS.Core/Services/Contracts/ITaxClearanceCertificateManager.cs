using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxClearanceCertificateManager<TaxClearanceCertificate> : IDependency, IBaseManager<TaxClearanceCertificate>
    {
        /// <summary>
        /// Gets tax clearance certificate with the specified TCC Number
        /// </summary>
        /// <param name="tccNumber"></param>
        /// <returns></returns>
        TCCertificateDetailsVM GetCertificateDetails(string tccNumber);
    }
}
