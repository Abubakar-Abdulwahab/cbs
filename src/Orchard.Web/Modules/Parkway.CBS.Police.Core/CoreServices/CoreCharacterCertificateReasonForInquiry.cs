using Orchard;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreCharacterCertificateReasonForInquiry : ICoreCharacterCertificateReasonForInquiry
    {
        private readonly ICharacterCertificateReasonForInquiryManager<CharacterCertificateReasonForInquiry> _repo;
        private readonly IOrchardServices _orchardServices;

        public CoreCharacterCertificateReasonForInquiry(ICharacterCertificateReasonForInquiryManager<CharacterCertificateReasonForInquiry> repo, IOrchardServices orchardServices)
        {
            _repo = repo;
            _orchardServices = orchardServices;
        }

        /// <summary>
        /// Get character certificate reasons for inquiry
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CharacterCertificateReasonForInquiryVM> GetReasonsForInquiry()
        {
            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;

            IEnumerable<CharacterCertificateReasonForInquiryVM> result = ObjectCacheProvider.GetCachedObject<IEnumerable<CharacterCertificateReasonForInquiryVM>>(tenant, $"{nameof(POSSAPCachePrefix.InquiryReasons)}");

            if (result == null)
            {
                result = _repo.GetReasonsForInquiry();

                if (result != null)
                {
                    ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.InquiryReasons)}", result);
                }
            }
            return result;
        }

        /// <summary>
        /// checks if certificate reason for inquiry exists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ValidateCertificateReasonForInquiry(int id)
        {
            return _repo.Count(x => x.Id == id) > 0;
        }

        /// <summary>
        /// Gets character certificate reason for inquiry with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<CharacterCertificateReasonForInquiryVM> GetReasonForInquiry(int id)
        {
            return _repo.GetReasonForInquiry(id);
        }
    }
}