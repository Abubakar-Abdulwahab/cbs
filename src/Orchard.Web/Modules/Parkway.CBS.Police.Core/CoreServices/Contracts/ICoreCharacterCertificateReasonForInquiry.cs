using Orchard;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreCharacterCertificateReasonForInquiry : IDependency
    {
        /// <summary>
        /// Get character certificate reasons for inquiry
        /// </summary>
        /// <returns></returns>
        IEnumerable<CharacterCertificateReasonForInquiryVM> GetReasonsForInquiry();

        /// <summary>
        /// checks if certificate reason for inquiry exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool ValidateCertificateReasonForInquiry(int id);

        /// <summary>
        /// Gets character certificate reason for inquiry with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<CharacterCertificateReasonForInquiryVM> GetReasonForInquiry(int id);
    }
}
