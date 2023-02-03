using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class CharacterCertificateReasonForInquiryManager : BaseManager<CharacterCertificateReasonForInquiry>, ICharacterCertificateReasonForInquiryManager<CharacterCertificateReasonForInquiry>
    {
        private readonly IRepository<CharacterCertificateReasonForInquiry> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public CharacterCertificateReasonForInquiryManager(IRepository<CharacterCertificateReasonForInquiry> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Gets all active reasons for character certificate inquiry
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CharacterCertificateReasonForInquiryVM> GetReasonsForInquiry()
        {
            try
            {
                return _transactionManager.GetSession().Query<CharacterCertificateReasonForInquiry>().Where(x => x.IsDeleted == false).Select(x => new CharacterCertificateReasonForInquiryVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    ShowFreeForm = x.FreeForm
                }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets character certificate reason for inquiry with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<CharacterCertificateReasonForInquiryVM> GetReasonForInquiry(int id)
        {
            try
            {
                return _transactionManager.GetSession().Query<CharacterCertificateReasonForInquiry>().Where(x => x.Id == id).Select(x => new CharacterCertificateReasonForInquiryVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    ShowFreeForm = x.FreeForm
                }).ToFuture();

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}