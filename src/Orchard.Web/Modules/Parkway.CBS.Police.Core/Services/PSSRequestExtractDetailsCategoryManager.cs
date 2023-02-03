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
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSRequestExtractDetailsCategoryManager : BaseManager<PSSRequestExtractDetailsCategory>, IPSSRequestExtractDetailsCategoryManager<PSSRequestExtractDetailsCategory>
    {
        private readonly IRepository<PSSRequestExtractDetailsCategory> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSRequestExtractDetailsCategoryManager(IRepository<PSSRequestExtractDetailsCategory> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets extract category names for display on extract document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public IEnumerable<string> GetExtractCategoriesForExtractDocument(string fileRefNumber)
        {
            try
            {
                char[] splitBy = new char[] { ':' };
                return _transactionManager.GetSession().Query<PSSRequestExtractDetailsCategory>()
                    .Where(x => x.Request.FileRefNumber == fileRefNumber)
                    .Select(x => (x.ExtractCategory.FreeForm) ? x.RequestReason.Split(splitBy)[1] : x.RequestReason.Split(splitBy)[0])
                    .ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets selected extract categories and sub categories for request with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public IEnumerable<PSSRequestExtractDetailsCategoryVM> GetExtractCategoriesForRequest(string fileRefNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSRequestExtractDetailsCategory>()
                    .Where(x => x.Request.FileRefNumber == fileRefNumber)
                    .Select(x => new PSSRequestExtractDetailsCategoryVM
                    {
                        ExtractCategoryId = x.ExtractCategory.Id,
                        ExtractSubCategoryId = x.ExtractSubCategory.Id,
                        RequestReason = x.RequestReason
                    }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting extract categories for request with file number" + fileRefNumber));
                throw;
            }
        }
    }
}