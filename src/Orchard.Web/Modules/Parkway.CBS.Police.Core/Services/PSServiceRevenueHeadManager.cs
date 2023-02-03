using Orchard;
using System.Linq;
using Orchard.Data;
using NHibernate.Linq;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSServiceRevenueHeadManager : BaseManager<PSServiceRevenueHead>, IPSServiceRevenueHeadManager<PSServiceRevenueHead>
    {

        private readonly IRepository<PSServiceRevenueHead> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public PSServiceRevenueHeadManager(IRepository<PSServiceRevenueHead> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="isApplicationInvoice"></param>
        /// <returns></returns>
        public IEnumerable<PSServiceRevenueHeadVM> GetRevenueHead(int serviceId, int levelId)
        {
            return _transactionManager.GetSession().Query<PSServiceRevenueHead>()
           .Where(x=> x.Service.Id == serviceId && x.FlowDefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = levelId } && x.IsActive)
           .Select(sr => new PSServiceRevenueHeadVM
           {
               ServiceName = sr.Service.Name,
               AmountToPay = sr.RevenueHead.BillingModel.Amount,
               FeeDescription = sr.Description,
               RevenueHeadId = sr.RevenueHead.Id,
               IsGroupHead = sr.RevenueHead.IsGroup,
               Surcharge = sr.RevenueHead.BillingModel.Surcharge
           }).ToFuture();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="appStage"></param>
        /// <returns>IEnumerable{PSServiceRevenueHeadVM}</returns>
        public IEnumerable<PSServiceRevenueHeadVM> GetRevenueHeadAndFormDetails(int serviceId, int levelId)
        {
            return _transactionManager.GetSession().Query<PSServiceRevenueHead>()
           .Where(x => x.Service.Id == serviceId && x.FlowDefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = levelId } && x.IsActive)
           .Select(sr => new PSServiceRevenueHeadVM
           {
               ServiceName = sr.Service.Name,
               RevenueHeadId = sr.RevenueHead.Id,
               Forms = sr.RevenueHead.FormControls.Select(f => new FormControlViewModel
               {
                   PartialName = f.Form.PartialName,
                   ControlIdentifier = f.Id,
                   HintText = f.Form.HintText,
                   PlaceHolderText = f.Form.PlaceHolderText,
                   FriendlyName = f.Form.FriendlyName,
                   LabelText = f.Form.LabelText,
                   PartialProvider = f.Form.PartialModelProvider,
                   TaxEntityCategoryId = f.TaxEntityCategory.Id,
                   TechnicalName = f.Form.TechnicalName,
                   Position = f.Position,
                   IsCompulsory = f.IsComplusory,
                   Validators = f.Form.Validators,
                   ValidationProps = f.Form.ValidationProps,
                   RevenueHeadId = f.RevenueHead.Id,
               }).ToList()
           }).ToFuture();
        }

    }
}