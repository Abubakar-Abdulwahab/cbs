using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;

namespace Parkway.CBS.Client.Web.Controllers.Handlers.Contracts
{
    public interface IPAYEWithNoScheduleUploadHandler : IDependency
    {

        /// <summary>
        /// Get generate invoice steps model for PAYE no schedule schedule upload
        /// </summary>
        /// <param name="userDetails"></param>
        /// <returns>GenerateInvoiceStepsModel</returns>
        GenerateInvoiceStepsModel GetPAYEDetailsForNoSchdeuleFileUpload(UserDetailsModel userDetails);


        /// <summary>
        /// When the schedule has been confirmed we move the records from the staging to the main table
        /// </summary>
        /// <param name="invoiceConfirmedModel">InvoiceConfirmedModel</param>
        /// <returns>string | BatchRef</returns>
        string ConfirmPAYESchedule(InvoiceConfirmedModel invoiceConfirmedModel);

    }
}
