using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class CharacterCertificateBiometricsManager : BaseManager<CharacterCertificateBiometrics>, ICharacterCertificateBiometricsManager<CharacterCertificateBiometrics>
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IRepository<CharacterCertificateBiometrics> _repository;
        private readonly IOrchardServices _orchardServices;

        public CharacterCertificateBiometricsManager(IRepository<CharacterCertificateBiometrics> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
        }

        /// <summary>
        /// Gets Character Certificate Biometrics by <paramref name="requestId"/>
        /// </summary>
        /// <returns></returns>
        public CharacterCertificateBiometricsVM GetCharacterCertificateBiometrics(long requestId)
        {
            try
            {
                return _transactionManager.GetSession().Query<CharacterCertificateBiometrics>().Where(x => x.Request == new PSSRequest { Id = requestId }).Select(x => new CharacterCertificateBiometricsVM
                {
                    Id = x.Id,
                    PassportImage = x.PassportImage,
                    FileNumber = x.Request.FileRefNumber,
                    RightIndex = x.RightIndex,
                    RegisteredAt = x.RegisteredAt,
                    RightMiddle = x.RightMiddle,
                    RightPinky = x.RightPinky,
                    RightRing = x.RightRing,
                    RightThumb = x.RightThumb,
                    LeftRing = x.LeftRing,
                    LeftIndex = x.LeftIndex,
                    LeftMiddle = x.LeftMiddle,
                    LeftPinky = x.LeftPinky,
                    LeftThumb = x.LeftThumb,
                    ApplicantName = x.Request.CBSUser.Name
                }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}