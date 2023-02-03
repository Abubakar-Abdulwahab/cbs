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
using Newtonsoft.Json;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSDispatchNoteManager : BaseManager<PSSDispatchNote>, IPSSDispatchNoteManager<PSSDispatchNote>
    {
        private readonly IRepository<PSSDispatchNote> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PSSDispatchNoteManager(IRepository<PSSDispatchNote> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Gets dispatch note with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public DispatchNoteVM GetDispatchNote(string fileRefNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSDispatchNote>().Where(x => x.FileRefNumber == fileRefNumber)
                    .Select(x => new DispatchNoteVM
                    {
                        ApplicantName = x.ApplicantName,
                        ApprovalNumber = x.ApprovalNumber,
                        FileNumber = x.FileRefNumber,
                        ServiceOriginLocation = (!string.IsNullOrEmpty(x.OriginAddress) && !string.IsNullOrEmpty(x.OriginLGAName) && !string.IsNullOrEmpty(x.OriginStateName)) ? $"{x.OriginAddress} {x.OriginLGAName}, {x.OriginStateName}" : "",
                        ServiceDeliveryLocation = $"{x.ServiceDeliveryAddress} {x.ServiceDeliveryLGAName}, {x.ServiceDeliveryStateName}",
                        ServiceDuration = $"{x.StartDate} - {x.EndDate}",
                        ServicingCommands = JsonConvert.DeserializeObject<IEnumerable<CommandVM>>(x.ServicingCommands),
                        PoliceOfficers = JsonConvert.DeserializeObject<IEnumerable<VM.PoliceOfficerLogVM>>(x.PoliceOfficers),
                        Template = x.DispatchNoteTemplate
                    }).SingleOrDefault();

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}