using System.Collections.Generic;
using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee.PayeeAdapters.ETCC;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEOnscreenHandler : IDependency
    {
        /// <summary>
        /// Do processing for onscreen PAYE schedule
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="payeLines"></param>
        /// <param name="userDetails"></param>
        void DoPAYEOnScreenProcessing(GenerateInvoiceStepsModel processStage, UserDetailsModel userDetails, ICollection<PAYEAssessmentLine> payeLines);

    }
}
