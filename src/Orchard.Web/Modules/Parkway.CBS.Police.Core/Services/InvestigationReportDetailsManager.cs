using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class InvestigationReportDetailsManager : BaseManager<InvestigationReportDetails>, IInvestigationReportDetailsManager<InvestigationReportDetails>
    {
        private readonly IRepository<InvestigationReportDetails> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public InvestigationReportDetailsManager(IRepository<InvestigationReportDetails> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get investigation report request view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns>IEnumerable<ExtractDetailsVM></returns>
        public IEnumerable<ExtractDetailsVM> GetInvestigationReportRequestViewDetails(string fileRefNumber, long taxEntityId)
        {
            try
            {
                return _transactionManager.GetSession().Query<InvestigationReportDetails>().Where(x => x.Request.FileRefNumber == fileRefNumber && x.Request.TaxEntity.Id == taxEntityId).Select(investigationReport => new ExtractDetailsVM
                {
                    TaxEntity = new TaxEntityViewModel
                    {
                        Recipient = investigationReport.Request.TaxEntity.Recipient,
                        PhoneNumber = investigationReport.Request.TaxEntity.PhoneNumber,
                        RCNumber = investigationReport.Request.TaxEntity.RCNumber,
                        Address = investigationReport.Request.TaxEntity.Address,
                        Email = investigationReport.Request.TaxEntity.Email,
                        TaxPayerIdentificationNumber = investigationReport.Request.TaxEntity.TaxPayerIdentificationNumber,
                        SelectedStateName = investigationReport.Request.TaxEntity.StateLGA.State.Name,
                        SelectedLGAName = investigationReport.Request.TaxEntity.StateLGA.Name
                    },

                    ExtractInfo = new ExtractRequestVM
                    {
                        Reason = investigationReport.RequestReason,
                        StateName = investigationReport.Request.Command.State.Name,
                        LGAName = investigationReport.Request.Command.LGA.Name,
                        CommandName = investigationReport.Request.Command.Name,
                        CommandAddress = investigationReport.Request.Command.Address
                    }
                }).ToFuture();
            }catch(Exception exception)
            {
                Logger.Error(exception, string.Format("Error getting investigation report details for request with file number" + fileRefNumber));
                throw;
            }
        }
    }
}