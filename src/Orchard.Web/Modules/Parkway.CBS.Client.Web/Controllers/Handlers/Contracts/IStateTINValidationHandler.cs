using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IStateTINValidationHandler : IDependency
    {
        /// <summary>
        /// Validate the existence of a particular stateTIN (payerid)
        /// </summary>
        /// <param name="stateTIN"></param>
        /// <returns>TaxPayerWithDetails</returns>
        TaxPayerWithDetails ValidateStateTIN(string stateTIN);
    }
}
