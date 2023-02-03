using System;
using Orchard;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.API.Controllers.Handlers.Contracts
{
    public interface IAPIRequestHandler : IDependency
    {

        /// <summary>
        /// this method checks to confirm that the present stage fee for this 
        /// request has been fully paid for
        /// </summary>
        /// <param name="requestToken"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        IEnumerable<PSServiceRequestInvoiceValidationDTO> ConfirmRequestInvoiceFee(string requestToken, string invoiceNumber);


        /// <summary>
        /// When the request fee has been confirmed, we update the service request status
        /// </summary>
        /// <param name="requestDeet"></param>
        void UpdateServiceRequest(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeet);


        /// <summary>
        /// Update the status log after payment has been confirmed
        /// </summary>
        /// <param name="requestDeet"></param>
        void UpdateRequestStatusLog(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeet);


        /// <summary>
        /// Here we are moving the request to the next definition level
        /// </summary>
        /// <param name="result"></param>
        APIResponse MoveRequestToTheNextDefinedLevel(IEnumerable<PSServiceRequestInvoiceValidationDTO> result);


        /// <summary>
        /// check if request can be moved to the next request stage
        /// </summary>
        /// <param name="result"></param>
        /// <param name="invoiceNumberGrp"></param>
        /// <returns>bool</returns>
        bool SkipRequestMoveToNextStage(IEnumerable<PSServiceRequestInvoiceValidationDTO> result, IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp);


        /// <summary>
        /// Do processing for request
        /// </summary>
        /// <param name="invoiceNumberGrp"></param>
        /// <param name="canMove"></param>
        /// <returns></returns>
        APIResponse DoProcessing(IEnumerable<PSServiceRequestInvoiceValidationDTO> invoiceNumberGrp, bool canMove);

    }
}
