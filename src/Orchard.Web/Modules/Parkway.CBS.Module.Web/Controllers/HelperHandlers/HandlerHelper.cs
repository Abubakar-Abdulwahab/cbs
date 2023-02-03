using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Entities.DTO;
using System;

namespace Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers
{
    public class HandlerHelper : IHandlerHelper
    {
        private readonly Lazy<ITenantStateSettings<TenantCBSSettings>> _tenantStateSettings;
        private readonly Lazy<IStateModelManager<StateModel>> _stateRepo;



        public HandlerHelper(Lazy<ITenantStateSettings<TenantCBSSettings>> tenantStateSettings, Lazy<IStateModelManager<StateModel>> stateRepo)
        {
            _tenantStateSettings = tenantStateSettings;
            _stateRepo = stateRepo;
        }


        /// <summary>
        /// Get the tenant for this request
        /// </summary>
        /// <returns></returns>
        public TenantCBSSettings GetTenant()
        {
            TenantCBSSettings tenant = _tenantStateSettings.Value.GetCollection(x => x.Id != 0).FirstOrDefault();
            if (tenant == null) { throw new TenantNotFoundException("Tenant setting not found"); }
            return tenant;
        }

        public List<StatesAndLGAs> GetAllStatesAndLgas()
        {
            return _stateRepo.Value.GetStatesAndLGAs();
        }

        public List<StateModel> GetAllStates()
        {
            return _stateRepo.Value.GetStates();
        }

        public List<LGA> GetStateLgas(int StateId) {

            return _stateRepo.Value.GetLgas(StateId);
        }

        public bool CheckLgaExists(int lgaId) {
            return _stateRepo.Value.ValidateLga(lgaId).Exists(n => n.Id == lgaId );
        }
    }
}