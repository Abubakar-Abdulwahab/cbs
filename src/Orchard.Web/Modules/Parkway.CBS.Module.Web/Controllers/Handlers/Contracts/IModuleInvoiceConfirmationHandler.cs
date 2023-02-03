using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.Web.Controllers.Handlers.Contracts
{
    public interface IModuleInvoiceConfirmationHandler : IDependency
    {

        CreateInvoiceModel GetCreateInvoiceModel(GenerateInvoiceStepsModel processStage, TaxEntity entity);


        /// <summary>
        /// Try generate invoice
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="entity"></param>
        /// <returns>string</returns>
        string TryGenerateInvoice(GenerateInvoiceStepsModel processStage, TaxEntity entity);

    }
}
