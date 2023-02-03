using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.IO;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSCharacterCertificateDetailsBlobManager : BaseManager<PSSCharacterCertificateDetailsBlob>, IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob>
    {
        private readonly IRepository<PSSCharacterCertificateDetailsBlob> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        private readonly IPSSCharacterCertificateDetailsLogManager<PSSCharacterCertificateDetailsLog> _pccLogManager;

        public PSSCharacterCertificateDetailsBlobManager(IRepository<PSSCharacterCertificateDetailsBlob> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices, IPSSCharacterCertificateDetailsLogManager<PSSCharacterCertificateDetailsLog> pccLogManager) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            _user = user;
            _pccLogManager = pccLogManager;
        }       

        /// <summary>
        /// Returns blob details for character certificate
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public PSSCharacterCertificateDetailsBlobVM GetCharacterCertificateBlobDetails(long requestId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSCharacterCertificateDetailsBlob>().Where(x => x.Request == new PSSRequest { Id = requestId }).Select(x => new PSSCharacterCertificateDetailsBlobVM
                {
                    PassportPhotographContentType = x.PassportPhotographContentType,
                    PassportPhotographBlob = x.PassportPhotographBlob
                }).SingleOrDefault();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Updates passport photo
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="characterCertificateDetailsId"></param>
        /// <returns></returns>
        public bool UpdatePassportPhoto(string filePathName, long characterCertificateDetailsId)
        {
            try
            {
                using (var session = _transactionManager.GetSession().SessionFactory.OpenSession())
                {
                    using (var tranx = session.BeginTransaction())
                    {
                        try
                        {
                            PSSCharacterCertificateDetailsBlob blob = session.Query<PSSCharacterCertificateDetailsBlob>().Where(x => x.PSSCharacterCertificateDetails == new PSSCharacterCertificateDetails { Id = characterCertificateDetailsId }).Single();

                            blob.UpdatedAtUtc = DateTime.Now.ToLocalTime();
                            blob.PassportPhotographBlob = Convert.ToBase64String(File.ReadAllBytes(filePathName));
                            blob.PassportPhotographContentType = Util.GetFileContentType(filePathName);
                            blob.PassportPhotographFilePath = filePathName;
                            blob.PassportPhotographOriginalFileName = Path.GetFileName(filePathName);

                            session.Flush();

                            var query = session.CreateSQLQuery(_pccLogManager.LogNewEntryQueryStringValue(characterCertificateDetailsId));
                            query.SetParameter("characterCertificateDetailsId", characterCertificateDetailsId);
                            query.ExecuteUpdate();

                            tranx.Commit();
                        }
                        catch (Exception exception)
                        {
                            Logger.Error(exception, string.Format("Could not save object "));
                            tranx.Rollback();
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception exception) { }
            return false;
        }


        /// <summary>
        /// Updates passport bio data page
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="characterCertificateDetailsId"></param>
        /// <returns></returns>
        public bool UpdatePassportBioDataPage(string filePathName, long characterCertificateDetailsId)
        {
            try
            {
                using (var session = _transactionManager.GetSession().SessionFactory.OpenSession())
                {
                    using (var tranx = session.BeginTransaction())
                    {
                        try
                        {
                            PSSCharacterCertificateDetailsBlob blob = session.Query<PSSCharacterCertificateDetailsBlob>().Where(x => x.PSSCharacterCertificateDetails == new PSSCharacterCertificateDetails { Id = characterCertificateDetailsId }).Single();

                            blob.UpdatedAtUtc = DateTime.Now.ToLocalTime();
                            blob.InternationalPassportDataPageBlob = Convert.ToBase64String(File.ReadAllBytes(filePathName));
                            blob.InternationalPassportDataPageContentType = Util.GetFileContentType(filePathName);
                            blob.InternationalPassportDataPageFilePath = filePathName;
                            blob.InternationalPassportDataPageOriginalFileName = Path.GetFileName(filePathName);

                            session.Flush();

                            var query = session.CreateSQLQuery(_pccLogManager.LogNewEntryQueryStringValue(characterCertificateDetailsId));
                            query.SetParameter("characterCertificateDetailsId", characterCertificateDetailsId);
                            query.ExecuteUpdate();

                            tranx.Commit();
                        }
                        catch (Exception exception)
                        {
                            Logger.Error(exception, string.Format("Could not save object "));
                            tranx.Rollback();
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception exception) 
            {
                Logger.Error(exception, $"Error updating bio data page. Exception message - {exception.Message}");
            }
            return false;
        }
    }
}