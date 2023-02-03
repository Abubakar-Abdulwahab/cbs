using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class TaxClearanceCertificateAuthorizedSignaturesManager : BaseManager<TaxClearanceCertificateAuthorizedSignatures>, ITaxClearanceCertificateAuthorizedSignaturesManager<TaxClearanceCertificateAuthorizedSignatures>
    {
        private readonly IRepository<TaxClearanceCertificateAuthorizedSignatures> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxClearanceCertificateAuthorizedSignaturesManager(IRepository<TaxClearanceCertificateAuthorizedSignatures> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Gets signature of the specified signatory
        /// </summary>
        /// <param name="signatory"></param>
        /// <returns></returns>
        public TCCAuthorizedSignatureVM GetAuthorizedSignatureOfSpecifiedSignatory(Models.Enums.TCCAuthorizedSignatories signatory)
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxClearanceCertificateAuthorizedSignatures>().Where(x => x.TCCAuthorizedSignatoryId == (int)signatory).Select(x => new TCCAuthorizedSignatureVM
                {
                    Id = x.Id,
                    BLOB = x.TCCAuthorizedSignatureBlob,
                    ContentType = x.ContentType
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