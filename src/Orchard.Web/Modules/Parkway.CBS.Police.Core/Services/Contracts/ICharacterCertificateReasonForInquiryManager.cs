using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ICharacterCertificateReasonForInquiryManager<CharacterCertificateReasonForInquiry> : IDependency, IBaseManager<CharacterCertificateReasonForInquiry>
    {
        /// <summary>
        /// Gets all active reasons for character certificate inquiry
        /// </summary>
        /// <returns></returns>
        IEnumerable<CharacterCertificateReasonForInquiryVM> GetReasonsForInquiry();

        /// <summary>
        /// Gets character certificate reason for inquiry with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<CharacterCertificateReasonForInquiryVM> GetReasonForInquiry(int id);
    }
}
