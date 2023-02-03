using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class ExtractApproval 
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Extract;
        private readonly Lazy<IExtractDetailsManager<ExtractDetails>> _extractDetailsManager;
        private readonly Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> _serviceRequestManager;
        public ILogger Logger { get; set; }
        private readonly Lazy<ITypeImplComposer> _compositionHandler;

        public ExtractApproval(Lazy<IExtractDetailsManager<ExtractDetails>> extractDetailsManager,Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> serviceRequestManager, Lazy<ITypeImplComposer> compositionHandler)
        {
            _extractDetailsManager = extractDetailsManager;
            Logger = NullLogger.Instance;
            _serviceRequestManager = serviceRequestManager;
            _compositionHandler = compositionHandler;
        }


        /// <summary>
        /// Save Extract or Escort approval details
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="sRequestFormDump"></param>
        /// <returns>RequestApprovalResponse</returns>
        public RequestApprovalResponse SaveRequestApprovalDetails(long requestId, dynamic requestVM)
        {
            throw new Exception { };
            //try
            //{
            //    PSServiceRequestInvoiceValidationDTO serviceDetails = _serviceRequestManager.Value.GetServiceRequestDetailsWithRequestId(requestId);
            //    if (serviceDetails == null)
            //    { throw new NoInvoicesMatchingTheParametersFoundException("404 for PSS application fee. Request Id " + requestId); }
            //    int currentStatus = serviceDetails.Request.Status;

            //    serviceDetails.ServiceRequest.Status = requestVM.ApprovalStatus;
            //    serviceDetails.ServiceRequest.UpdatedAtUtc = DateTime.Now;
            //    serviceDetails.Request.Status = requestVM.ApprovalStatus;
            //    serviceDetails.Request.ApprovedBy = new UserPartRecord { Id = requestVM.ApproverId };
            //    serviceDetails.Request.Comment = requestVM.Comment;
            //    if (serviceDetails.Request.Status == (int)PSSRequestStatus.Approved)
            //    {
            //        serviceDetails.Request.ApprovalNumber = Util.ZeroPadUp(requestId.ToString(), 10, $"PSS_{DateTime.Now.ToString("MMdd")}");
            //    }
            //    var approvalDetails = new PoliceRequestSatusLog
            //    {
            //        StatusBefore = currentStatus,
            //        StatusAfter = requestVM.ApprovalStatus,
            //        Comment = requestVM.Comment,
            //        Request = new PSSRequest { Id = requestId },
            //        UpdatedAtUtc = DateTime.Now,
            //        AdminUser = new UserPartRecord { Id = requestVM.ApproverId }
            //    };

            //    _compositionHandler.Value.SaveApproverDetails(approvalDetails);
            //    TaxEntity taxEntity = new TaxEntity
            //    {
            //        Recipient = serviceDetails.Request.TaxEntity.Recipient,
            //        Id = serviceDetails.Request.TaxEntity.Id,
            //        Email = serviceDetails.Request.TaxEntity.Email
            //    };

            //    if(requestVM.ApprovalStatus == (int)PSSRequestStatus.Approved)
            //    {
            //        _compositionHandler.Value.SendEmailNotification(taxEntity, new EmailDetailVM  { ApprovalStatus  = requestVM.ApprovalStatus, ApprovalNumber = serviceDetails.Request.ApprovalNumber, TemplateName = PulseTemplateFileNames.ExtractApprovalNotification.ToDescription(), Subject = "Police Service Extract Approval Notification", InvoiceNumber = serviceDetails.InvoiceNumber, RequestType = "Extract Request", RequestDate = serviceDetails.Request.CreatedAtUtc.ToString("dd MMM yyyy") });
            //    }
            //    else
            //    {
            //        _compositionHandler.Value.SendEmailNotification(taxEntity, new EmailDetailVM { ApprovalStatus = requestVM.ApprovalStatus, Comment = requestVM.Comment, TemplateName = PulseTemplateFileNames.ExtractRejectionNotification.ToDescription(), Subject = "Police Service Extract Rejection Notification", RequestDate = serviceDetails.Request.CreatedAtUtc.ToString("dd MMM yyyy") });
            //    }

            //    return new RequestApprovalResponse { CustomerName = taxEntity.Recipient, ServiceType = serviceDetails.ServiceType.ToString(), FileNumber = serviceDetails.Request.FileRefNumber, ApprovalNumber = serviceDetails.Request.ApprovalNumber };
            //}
            //catch (Exception exception)
            //{
            //    Logger.Error(exception, string.Format("{0}", exception.Message));
            //    _serviceRequestManager.Value.RollBackAllTransactions();
            //    throw;
            //}
        }
    }
}