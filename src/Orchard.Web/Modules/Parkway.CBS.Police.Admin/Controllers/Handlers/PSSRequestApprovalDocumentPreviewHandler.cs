using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class PSSRequestApprovalDocumentPreviewHandler : IPSSRequestApprovalDocumentPreviewHandler
    {
        private readonly IEnumerable<IPSSServiceTypeDocumentPreviewImpl> _pssServiceTypeDocumentPreviewImpl;
        private readonly IPSSRequestManager<PSSRequest> _requestManager;
        private readonly IPSSRequestApprovalDocumentPreviewLogManager<PSSRequestApprovalDocumentPreviewLog> _pssRequestApprovalDocumentPreviewLogManager;
        ILogger Logger { get; set; }
        public PSSRequestApprovalDocumentPreviewHandler(IEnumerable<IPSSServiceTypeDocumentPreviewImpl> pssServiceTypeDocumentPreviewImpl, IPSSRequestManager<PSSRequest> requestManager, IPSSRequestApprovalDocumentPreviewLogManager<PSSRequestApprovalDocumentPreviewLog> pssRequestApprovalDocumentPreviewLogManager)
        {
            _pssServiceTypeDocumentPreviewImpl = pssServiceTypeDocumentPreviewImpl;
            _pssRequestApprovalDocumentPreviewLogManager = pssRequestApprovalDocumentPreviewLogManager;
            _requestManager = requestManager;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Generate draft service document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        public CreateCertificateDocumentVM CreateDraftServiceDocument(string fileRefNumber)
        {
            try
            {
                int serviceType = _requestManager.GetServiceType(fileRefNumber);
                foreach(var impl in _pssServiceTypeDocumentPreviewImpl)
                {
                    if(impl.GetServiceTypeDefinition == (PSSServiceTypeDefinition)serviceType)
                    {
                        return impl.CreateDraftServiceDocumentByteFile(fileRefNumber);
                    }
                }
                throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + serviceType);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Confirms the admin user has viewed the service specific draft document during approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        public APIResponse ConfirmAdminHasViewedDraftDocument(string fileRefNumber, int adminId)
        {
            try
            {
                CreateCertificateDocumentVM serviceDraftDocument = null;
                if(string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0) { throw new Exception("File ref number not specified"); }
                var request = _requestManager.GetRequestDetails(fileRefNumber);
                if (request == null) { throw new Exception($"404 no request found for file ref number {fileRefNumber}"); }
                if(_pssRequestApprovalDocumentPreviewLogManager.Count(x => x.Request == new PSSRequest { Id = request.Id } && x.Approver == new Orchard.Users.Models.UserPartRecord { Id = adminId } && x.FlowDefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = request.FlowDefinitionLevelId } && x.Confirmed) > 0)
                {
                    //This means that the approver has already confirmed that he/she has viewed the draft document for this level and does not need to confirm again
                    return new APIResponse { ResponseObject = "Confirmed" };
                }

                foreach (var impl in _pssServiceTypeDocumentPreviewImpl)
                {
                    if (impl.GetServiceTypeDefinition == (PSSServiceTypeDefinition)request.ServiceTypeId)
                    {
                        serviceDraftDocument = impl.CreateDraftServiceDocumentByteFile(fileRefNumber);
                    }
                }

                if(serviceDraftDocument == null) { throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + request.ServiceTypeId); }
                
                PSSRequestApprovalDocumentPreviewLog documentPreviewLog = new PSSRequestApprovalDocumentPreviewLog
                {
                    Request = new PSSRequest { Id = request.Id },
                    Approver = new Orchard.Users.Models.UserPartRecord { Id = adminId},
                    FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = request.FlowDefinitionLevelId },
                    RequestDocumentDraftBlob = Convert.ToBase64String(serviceDraftDocument.DocByte),
                    Confirmed = true,
                };

                if (!_pssRequestApprovalDocumentPreviewLogManager.Save(documentPreviewLog)) { throw new CouldNotSaveRecord(); }

                return new APIResponse { ResponseObject = "Confirmed" };

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _pssRequestApprovalDocumentPreviewLogManager.RollBackAllTransactions();
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text };
            }
        }
    }
}