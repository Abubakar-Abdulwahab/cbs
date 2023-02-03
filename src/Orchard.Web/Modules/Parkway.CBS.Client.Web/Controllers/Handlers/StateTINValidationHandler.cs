using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Client.Web.Controllers.Handlers
{
    public class StateTINValidationHandler : IStateTINValidationHandler
    {
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityManager<TaxEntity> _taxEntityManager;

        public StateTINValidationHandler(IOrchardServices orchardServices, ITaxEntityManager<TaxEntity> taxEntityManager)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _taxEntityManager = taxEntityManager;
        }

        /// <summary>
        /// Validate the existence of a particular stateTIN (payerid)
        /// </summary>
        /// <param name="stateTIN"></param>
        /// <returns>TaxPayerWithDetails</returns>
        public TaxPayerWithDetails ValidateStateTIN(string stateTIN)
        {
            try
            {
                //
                var taxProfileDetails = _taxEntityManager.GetTaxPayerWithDetails(stateTIN);
                if (taxProfileDetails == null) { throw new NoRecordFoundException("No record found for State TIN " + stateTIN); }

                return taxProfileDetails;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw;
            }
        }

    }
}