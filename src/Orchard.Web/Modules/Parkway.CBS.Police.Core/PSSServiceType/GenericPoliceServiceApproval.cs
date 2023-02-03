using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class GenericPoliceServiceApproval
    {
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.GenericPoliceServices;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly Lazy<ITypeImplComposer> _compositionHandler;
        private readonly Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> _serviceRequestManager;
        private readonly Lazy<IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue>> _formControlRevenueHeadValueManager;


        public GenericPoliceServiceApproval(IOrchardServices orchardServices, Lazy<ITypeImplComposer> compositionHandler, Lazy<IPoliceServiceRequestManager<PoliceServiceRequest>> serviceRequestManager, Lazy<IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue>> formControlRevenueHeadValueManager)
        {
            _compositionHandler = compositionHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _formControlRevenueHeadValueManager = formControlRevenueHeadValueManager;
            _serviceRequestManager = serviceRequestManager;
        }

        /// <summary>
        /// Get the generic request view details using request id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public dynamic GetServiceRequestViewDetails(long requestId)
        {
            throw new Exception { };
            //PSServiceRequestInvoiceValidationDTO serviceDetails = _serviceRequestManager.Value.GetServiceRequestDetailsWithRequestId(requestId);
            //if (serviceDetails == null)
            //{ throw new NoInvoicesMatchingTheParametersFoundException("404 for PSS request. Request Id " + requestId); }

            //return new GenericRequestDetailsVM
            //{
            //    TaxEntity = new TaxEntityViewModel
            //    {
            //        Recipient = serviceDetails.Request.TaxEntity.Recipient,
            //        PhoneNumber = serviceDetails.Request.TaxEntity.PhoneNumber,
            //        RCNumber = serviceDetails.Request.TaxEntity.RCNumber,
            //        Address = serviceDetails.Request.TaxEntity.Address,
            //        Email = serviceDetails.Request.TaxEntity.Email,
            //        TaxPayerIdentificationNumber = serviceDetails.Request.TaxEntity.TaxPayerIdentificationNumber,
            //        SelectedStateName = serviceDetails.Request.TaxEntity.StateLGA.State.Name,
            //        SelectedLGAName = serviceDetails.Request.TaxEntity.StateLGA.Name
            //    },

            //    FormControlValues = _formControlRevenueHeadValueManager.Value.GetRevenueHeadFormControlValues(serviceDetails.ServiceRequest.Invoice.Id),
            //    RequestId = serviceDetails.Request.Id,
            //    ServiceTypeId = serviceDetails.ServiceRequest.Service.ServiceType,
            //    ActionName = "PSSGenericDetails",
            //    ServiceName = serviceDetails.ServiceRequest.Service.Name
            //};
        }

        /// <summary>
        /// Save Generic request approval details
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="requestVM"></param>
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
            //    serviceDetails.Request.Status = requestVM.ApprovalStatus;
            //    serviceDetails.Request.ApprovedBy = new UserPartRecord { Id = requestVM.ApproverId };
            //    serviceDetails.Request.Comment = requestVM.Comment;
            //    var approvalDetails = new PoliceRequestSatusLog
            //    {
            //        StatusBefore = currentStatus,
            //        StatusAfter = requestVM.ApprovalStatus,
            //        Comment = requestVM.Comment,
            //        Request = new PSSRequest { Id = requestId },
            //        UpdatedAtUtc = DateTime.Now,
            //        AdminUser = new UserPartRecord { Id = requestVM.ApproverId }
            //    };

            //    TaxEntity taxEntity = new TaxEntity
            //    {
            //        Recipient = serviceDetails.Request.TaxEntity.Recipient,
            //        Id = serviceDetails.Request.TaxEntity.Id,
            //        Email = serviceDetails.Request.TaxEntity.Email,
            //        TaxEntityCategory = new TaxEntityCategory { Id = serviceDetails.Request.TaxEntity.TaxEntityCategory.Id }
            //    };

            //    if (requestVM.ApprovalStatus == (int)PSSRequestStatus.Rejected)
            //    {
            //        //The status of the service request will change to rejected if the request is rejected
            //        serviceDetails.ServiceRequest.Status = (int)PSSRequestStatus.Rejected;
            //        _compositionHandler.Value.SaveApproverDetails(approvalDetails);
            //        //Log the email notification
            //        _compositionHandler.Value.SendEmailNotification(taxEntity, new EmailDetailVM { ApprovalStatus = requestVM.ApprovalStatus, Comment = requestVM.Comment, TemplateName = PulseTemplateFileNames.GenericRejectionNotification.ToDescription(), RequestType = $"{serviceDetails.ServiceRequest.Service.Name}", Subject = $"{serviceDetails.ServiceRequest.Service.Name} Rejection Notification", RequestDate = serviceDetails.Request.CreatedAtUtc.ToString("dd MMM yyyy") });
            //        return new RequestApprovalResponse { CustomerName = taxEntity.Recipient, ServiceType = serviceDetails.ServiceRequest.Service.Name, FileNumber = serviceDetails.Request.FileRefNumber };
            //    }


            //    //get the revenue head(s)
            //    InvoiceGenerationResponse response = null;
            //    IEnumerable<PSServiceRevenueHeadVM> results = _compositionHandler.Value.GetRevenueHeadDetails(serviceDetails.ServiceId, (int)PSSRevenueServiceStep.RequestFee);
            //    if (results != null && results.Any())
            //    {
            //        //Generate invoice for request fee
            //        //The status of the service request will change to paid if the request is approved
            //        serviceDetails.ServiceRequest.Status = (int)PSSRequestStatus.Paid;
            //        response = GenerateRequestInvoiceAfterApproval(results, serviceDetails, taxEntity);
            //        _compositionHandler.Value.CreateRequestsForApproval(results, serviceDetails, response.InvoiceId);
            //    }

            //    _compositionHandler.Value.SaveApproverDetails(approvalDetails);
            //    //Log the notification to an email
            //    if (requestVM.ApprovalStatus == (int)PSSRequestStatus.Approved)
            //    {
            //        _compositionHandler.Value.SendEmailNotification(taxEntity, new EmailDetailVM { ApprovalStatus = requestVM.ApprovalStatus, TemplateName = PulseTemplateFileNames.GenericApprovalNotification.ToDescription(), Subject = $"{serviceDetails.ServiceRequest.Service.Name} Approval Notification", InvoiceNumber = response != null? response.InvoiceNumber : serviceDetails.InvoiceNumber, RequestType = $"{serviceDetails.ServiceRequest.Service.Name}", RequestDate = serviceDetails.Request.CreatedAtUtc.ToString("dd MMM yyyy") });
            //    }

            //    return new RequestApprovalResponse { CustomerName = serviceDetails.Request.TaxEntity.Recipient, ServiceType = serviceDetails.ServiceRequest.Service.Name, FileNumber = serviceDetails.Request.FileRefNumber };
            //}
            //catch (Exception exception)
            //{
            //    Logger.Error(exception, string.Format("{0}", exception.Message));
            //    _serviceRequestManager.Value.RollBackAllTransactions();
            //    throw;
            //}
        }

        /// <summary>
        /// Save request details for this request
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="sRequestFormDump"></param>
        /// <param name="taxPayerProfileVM"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        private InvoiceGenerationResponse GenerateRequestInvoiceAfterApproval(IEnumerable<PSServiceRevenueHeadVM> serviceRevenueHeadVMs, PSServiceRequestInvoiceValidationDTO pSServiceRequest, TaxEntity taxEntity)
        {
            try
            {
                //get the revenue head
                IEnumerable<ExpertSystemVM> expertSystem = _compositionHandler.Value.GetExpertSystem();

                PSServiceRevenueHeadVM parentServicerevenueHead = serviceRevenueHeadVMs.Count() > 1 ? serviceRevenueHeadVMs.Where(r => r.IsGroupHead).Single() : serviceRevenueHeadVMs.ElementAt(0);

                TaxEntityViewModel entityVM = new TaxEntityViewModel { Id = taxEntity.Id, CategoryId = taxEntity.TaxEntityCategory.Id };
                CreateInvoiceUserInputModel inputModel = GetInvoiceUserInputModel(parentServicerevenueHead, pSServiceRequest.Request.Id, serviceRevenueHeadVMs, entityVM.CategoryId, pSServiceRequest.Request.FileRefNumber);

                InvoiceGenerationResponse response = _compositionHandler.Value.GenerateInvoice(inputModel, expertSystem.ElementAt(0), entityVM);
                response.PaymentURL = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == TenantConfigKeys.BaseURL.ToString()).FirstOrDefault().Value + "/p/make-payment/" + response.InvoiceNumber;

                return response;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("{0}", exception.Message));
                _serviceRequestManager.Value.RollBackAllTransactions();
                throw;
            }
        }

        /// <summary>
        /// create invoice input model
        /// </summary>
        /// <param name="parentServiceRevenueHead"></param>
        /// <param name="requestId"></param>
        /// <param name="result"></param>
        /// <param name="categoryId"></param>
        /// <param name="fileRefNumber"></param>
        /// <returns>CreateInvoiceUserInputModel</returns>
        private CreateInvoiceUserInputModel GetInvoiceUserInputModel(PSServiceRevenueHeadVM parentServiceRevenueHead, long requestId, IEnumerable<PSServiceRevenueHeadVM> result, int categoryId, string fileRefNumber)
        {
            return new CreateInvoiceUserInputModel
            {
                GroupId = parentServiceRevenueHead.IsGroupHead ? parentServiceRevenueHead.RevenueHeadId : 0,
                InvoiceTitle = parentServiceRevenueHead.FeeDescription,
                InvoiceDescription = string.Format("{0} for {1} {2}", parentServiceRevenueHead.FeeDescription, parentServiceRevenueHead.ServiceName, fileRefNumber),
                CallBackURL = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == TenantConfigKeys.RequestFeeAPICallBack.ToString()).FirstOrDefault().Value + "/?requestToken=" + requestId.ToString(),
                TaxEntityCategoryId = categoryId,
                RevenueHeadModels = result.Where(r => !r.IsGroupHead).Select(r =>
                new RevenueHeadUserInputModel
                {
                    AdditionalDescription = string.Format("{0} for {1} {2}", r.FeeDescription, r.ServiceName, fileRefNumber),
                    Amount = r.AmountToPay,
                    Quantity = 1,
                    RevenueHeadId = r.RevenueHeadId
                }).ToList()
            };
        }
    }
}