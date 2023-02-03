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
using System.Web;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSExtractDocumentManager : BaseManager<PSSExtractDocument>, IPSSExtractDocumentManager<PSSExtractDocument>
    {
        private readonly IRepository<PSSExtractDocument> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSSExtractDocumentManager(IRepository<PSSExtractDocument> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets extract document details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public ExtractDocumentVM GetExtractDocumentDetails(string fileRefNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSExtractDocument>().Where(x => x.ExtractDetails.Request.FileRefNumber == fileRefNumber).Select(x => new ExtractDocumentVM
                {
                    CommandName = x.CommandName,
                    CommandStateName = x.CommandStateName,
                    ApprovalDate = x.UpdatedAtUtc.Value,
                    ApprovalNumber = x.ApprovalNumber,
                    DiarySerialNumber = x.DiarySerialNumber,
                    IncidenDateAndTimeParsed = x.IncidenDateAndTime,
                    ExtractCategoriesConcat = x.ExtractCategories,
                    CrossRef = x.CrossRef,
                    Content = x.Content,
                    DPOName = x.DPOName,
                    Template = x.ExtractDocumentTemplate,
                    DPORankCode = x.DPORankCode,
                    DPOSignatureBlob = x.DPOSignatureBlob,
                    DPOSignatureContentType = x.DPOSignatureContentType
                }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}