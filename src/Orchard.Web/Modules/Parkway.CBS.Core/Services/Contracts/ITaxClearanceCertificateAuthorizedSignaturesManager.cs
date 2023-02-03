using Orchard;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITaxClearanceCertificateAuthorizedSignaturesManager<TaxClearanceCertificateAuthorizedSignatures> : IDependency, IBaseManager<TaxClearanceCertificateAuthorizedSignatures>
    {
        /// <summary>
        /// Gets signature of the specified signatory
        /// </summary>
        /// <param name="signatory"></param>
        /// <returns></returns>
        TCCAuthorizedSignatureVM GetAuthorizedSignatureOfSpecifiedSignatory(Models.Enums.TCCAuthorizedSignatories signatory);
    }
}
