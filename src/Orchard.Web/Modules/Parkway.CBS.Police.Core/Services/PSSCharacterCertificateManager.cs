using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSCharacterCertificateManager : BaseManager<PSSCharacterCertificate>, IPSSCharacterCertificateManager<PSSCharacterCertificate>
    {
        private readonly IRepository<PSSCharacterCertificate> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSSCharacterCertificateManager(IRepository<PSSCharacterCertificate> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }       

        /// <summary>
        /// Gets character certificate details with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public CharacterCertificateDocumentVM GetCertificateDetails(string fileRefNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSCharacterCertificate>()
               .Where(x => x.PSSCharacterCertificateDetails.Request.FileRefNumber == fileRefNumber)
               .Select(x => new CharacterCertificateDocumentVM
               {
                   ApprovalNumber = x.ApprovalNumber,
                   PassportPhotoContentType = x.PassportPhotoContentType,
                   PassportPhotoBlob = x.PassportPhotoBlob,
                   RefNumber = x.RefNumber,
                   DateOfApproval = x.DateOfApproval,
                   CustomerName = x.CustomerName,
                   CountryOfPassport = (x.CountryOfPassport == null) ? "" : x.CountryOfPassport,
                   PassportNumber = (x.PassportNumber == null) ? "" : x.PassportNumber,
                   PlaceOfIssuance = (x.PlaceOfIssuance == null) ? "" : x.PlaceOfIssuance,
                   DateOfIssuance = x.DateOfIssuance,
                   ReasonForInquiry = x.ReasonForInquiry,
                   DestinationCountry = (x.DestinationCountry == null) ? "" : x.DestinationCountry,
                   Template = x.CharacterCertificateTemplate,
                   CPCCRName = x.PSSCharacterCertificateDetails.CPCCRName,
                   RequestType = x.PSSCharacterCertificateDetails.RequestType.Name,
                   CPCCRRankCode = x.CPCCRRankCode,
                   CPCCRRankName = x.CPCCRRankName,
                   CPCCRSignatureBlob = x.CentralRegistrarSignatureBlob,
                   CPCCRSignatureContentType = x.CentralRegistrarSignatureContentType
               }).SingleOrDefault();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}