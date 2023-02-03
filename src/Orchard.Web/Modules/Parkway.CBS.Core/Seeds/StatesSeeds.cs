using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Seeds.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Seeds
{
    public class StatesSeeds : IStatesSeeds
    {
        public IInvoicingService _invoicingService;
        private readonly IStateModelManager<StateModel> _repo;

        public StatesSeeds(IInvoicingService invoicingService, IStateModelManager<StateModel> repo)
        {
            _invoicingService = invoicingService;
            _repo = repo;
        }


        public void PopulateStates()
        {
            var states = ListOfStates(null);
            var stVal = states.Select(s => new StateModel { Name = s.Name, ShortName = s.Name.Substring(0,3).ToUpper() });
            _repo.SaveBundle(stVal.ToList());
        }

        public List<CashFlowState> ListOfStates(CashFlowRequestContext context)
        {
            //Logger.Information("Getting list of states from cashflow");
            if (context == null) { context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } }); }
            var statesService = _invoicingService.StateService(context);
            return statesService.ListOfStates();
        }
    }
}