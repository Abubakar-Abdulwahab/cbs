using System;
using Orchard;
using System.Linq;
using Orchard.Data;
using Orchard.Logging;
using NHibernate.Linq;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.Services
{
    public class PoliceServiceRequestManager : BaseManager<PoliceServiceRequest>, IPoliceServiceRequestManager<PoliceServiceRequest>
    {
        private readonly IRepository<PoliceServiceRequest> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }


        public PoliceServiceRequestManager(IRepository<PoliceServiceRequest> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        /// <summary>
        /// Get request details for request with form inputs
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>GenericRequestDetailsVM</returns>
        public GenericRequestDetailsVM GetServiceRequestDetailsForGenericWithRequestId(long requestId)
        {
            return _transactionManager.GetSession().Query<PoliceServiceRequest>()
                .Where(sr => (sr.Request == new PSSRequest { Id = requestId } && (sr.Request.FlowDefinitionLevel == sr.FlowDefinitionLevel)))
                .Select(sr => new GenericRequestDetailsVM
                {
                    TaxEntity = new TaxEntityViewModel
                    {
                        Recipient = sr.Request.TaxEntity.Recipient,
                        PhoneNumber = sr.Request.TaxEntity.PhoneNumber,
                        RCNumber = sr.Request.TaxEntity.RCNumber,
                        Address = sr.Request.TaxEntity.Address,
                        Email = sr.Request.TaxEntity.Email,
                        TaxPayerIdentificationNumber = sr.Request.TaxEntity.TaxPayerIdentificationNumber,
                        SelectedStateName = sr.Request.TaxEntity.StateLGA.State.Name,
                        SelectedLGAName = sr.Request.TaxEntity.StateLGA.Name
                    },
                    Reason = sr.Request.Reason,
                    StateName = sr.Request.Command.State.Name,
                    LGAName = sr.Request.Command.LGA.Name,
                    CommandName = sr.Request.Command.Name,
                    CommandAddress = sr.Request.Command.Address,
                    RequestId = sr.Request.Id,
                    ServiceTypeId = sr.Request.Service.ServiceType,
                    ServiceName = sr.Request.Service.Name,
                    FileRefNumber = sr.Request.FileRefNumber,
                    Status = sr.Request.Status,
                    ApprovalButtonName = sr.Request.FlowDefinitionLevel.ApprovalButtonName,
                    ApprovalPartialName = sr.Request.FlowDefinitionLevel.PartialName,
                    CbsUser = new CBSUserVM { Name = sr.Request.CBSUser.Name, PhoneNumber = sr.Request.CBSUser.PhoneNumber, Email = sr.Request.CBSUser.Email }
                }).ToList().SingleOrDefault();
        }



        /// <summary>
        /// Get the invoice details as it pertains to service request table using the request Id, 
        /// with the matching flow definition level
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        public IEnumerable<PSServiceRequestInvoiceValidationDTO> GetServiceRequestDetailsWithRequestId(Int64 requestId)
        {
            return _transactionManager.GetSession().Query<PoliceServiceRequest>()
                .Where(sr => (sr.Request == new PSSRequest { Id = requestId } && (sr.Request.FlowDefinitionLevel == sr.FlowDefinitionLevel)))
                .Select(sr => new PSServiceRequestInvoiceValidationDTO
                {
                    AmountDue = sr.Invoice.InvoiceAmountDueSummary.AmountDue,
                    InvoiceStatus = (InvoiceStatus)sr.Invoice.Status,
                    Request = new PSSRequest { Id = sr.Request.Id, FileRefNumber = sr.Request.FileRefNumber, Status = sr.Request.Status },
                    PaymentDate = sr.Invoice.PaymentDate,
                    CancellationDate = sr.Invoice.CancelDate,
                    ServiceId = sr.Service.Id,
                    ServiceType = (PSSServiceTypeDefinition)sr.Service.ServiceType,
                    InvoiceNumber = sr.Invoice.InvoiceNumber,
                    DefinitionId = sr.FlowDefinitionLevel.Definition.Id,
                    DefinitionLevelId = sr.FlowDefinitionLevel.Id,
                    DefinitionLevelIdPosition = sr.FlowDefinitionLevel.Position,
                    TaxEntityCategoryId = sr.Request.TaxEntity.TaxEntityCategory.Id,
                    TaxEntityId = sr.Request.TaxEntity.Id,
                    ExpertSystemId = sr.Invoice.ExpertSystemSettings.Id,
                    InvoiceId = sr.Invoice.Id,
                    ServiceRequestStatus = sr.Status,
                    PhoneNumber = string.IsNullOrEmpty(sr.Request.ContactPersonPhoneNumber) ? sr.Request.CBSUser.PhoneNumber : sr.Request.ContactPersonPhoneNumber,
                    Recipient = sr.Request.CBSUser.Name,
                    Email = string.IsNullOrEmpty(sr.Request.ContactPersonEmail) ? sr.Request.CBSUser.Email : sr.Request.ContactPersonEmail,
                    ServiceName = sr.Service.Name,
                    CommandName = sr.Request.Command.Name,
                    CommandAddress = sr.Request.Command.Address,
                    CommandId = sr.Request.Command.Id
                });
        }


        /// <summary>
        /// Get generic police request view details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="taxEntityId"></param>
        /// <returns></returns>
        public IEnumerable<GenericRequestDetailsVM> GetGenericServiceRequestDetails(string fileRefNumber, long taxEntityId)
        {
            return _transactionManager.GetSession().Query<PoliceServiceRequest>()
                .Where(gr => gr.Request.FileRefNumber == fileRefNumber && gr.Request.TaxEntity.Id == taxEntityId)
                .Select(gr => new GenericRequestDetailsVM
                {
                    TaxEntity = new TaxEntityViewModel
                    {
                        Recipient = gr.Request.TaxEntity.Recipient,
                        PhoneNumber = gr.Request.TaxEntity.PhoneNumber,
                        RCNumber = gr.Request.TaxEntity.RCNumber,
                        Address = gr.Request.TaxEntity.Address,
                        Email = gr.Request.TaxEntity.Email,
                        TaxPayerIdentificationNumber = gr.Request.TaxEntity.TaxPayerIdentificationNumber,
                        SelectedStateName = gr.Request.TaxEntity.StateLGA.State.Name,
                        SelectedLGAName = gr.Request.TaxEntity.StateLGA.Name,
                    },
                    RequestId = gr.Request.Id,
                    ServiceTypeId = gr.Service.ServiceType,
                    ServiceName = gr.Service.Name,
                    CommandName = gr.Request.Command.Name,
                    CommandStateName = gr.Request.Command.State.Name,
                    CommandLgaName = gr.Request.Command.LGA.Name,
                    FileRefNumber = gr.Request.FileRefNumber,
                    ApprovalNumber = gr.Request.ApprovalNumber,
                    Status = gr.Request.Status
                }).ToFuture();
        }


        public ReceiptDisplayVM GetReceipts(string invoiceNumber)
        {
            try
            {
                ReceiptDisplayVM invoiceDetails = _transactionManager.GetSession().Query<PoliceServiceRequest>()
                        .Where(inv => inv.Invoice.InvoiceNumber == invoiceNumber)
                        .Select(inv => new ReceiptDisplayVM()
                        {
                            PayerId = inv.Invoice.TaxPayer.PayerId,
                            Email = inv.Invoice.TaxPayer.Email,
                            PhoneNumber = inv.Invoice.TaxPayer.PhoneNumber,
                            Recipient = inv.Invoice.TaxPayer.Recipient,
                            ServiceName = inv.Request.Service.Name,
                            TIN = inv.Invoice.TaxPayer.TaxPayerIdentificationNumber,
                            AmountDue = inv.Invoice.InvoiceAmountDueSummary.AmountDue,
                            FileNumber = inv.Request.FileRefNumber,
                            InvoiceNumber = inv.Invoice.InvoiceNumber,
                            Address = inv.Invoice.TaxPayer.Address,
                            InvoiceDesc = inv.Invoice.InvoiceDescription,
                            Transactions = inv.Invoice.Payments.Select(p => new InvoicePaymentsVM
                            {
                                AmountPaid = p.AmountPaid,
                                PaymentReference = p.PaymentReference,
                                ReceiptNumber = p.Receipt.ReceiptNumber,
                                PaymentDate = p.CreatedAtUtc.ToString("dd MMMM yyyy"),
                                TypeID = p.TypeID,
                                InvoiceDesc = p.RevenueHead.Name
                            }).ToList()
                        }).ToList().FirstOrDefault();

                if (invoiceDetails != null && invoiceDetails.Transactions.Any())
                {
                    invoiceDetails.Transactions = invoiceDetails.Transactions.DefaultIfEmpty().Where(p => p.TypeID == (int)PaymentType.Credit).ToList();
                }

                return invoiceDetails;

            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }


        /// <summary>
        /// Get receipt details
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="receiptNumber"></param>
        /// <returns>ReceiptDetailsVM</returns>
        public ReceiptDetailsVM GetReceipt(string invoiceNumber, string receiptNumber)
        {
            try
            {
                ReceiptDetailsVM invoiceDetails = _transactionManager.GetSession().Query<PoliceServiceRequest>()
                        .Where(inv => inv.Invoice.InvoiceNumber == invoiceNumber)
                        .Select(inv => new ReceiptDetailsVM()
                        {
                            PayerId = inv.Invoice.TaxPayer.PayerId,
                            Email = inv.Invoice.TaxPayer.Email,
                            ServiceName = inv.Request.Service.Name,
                            PhoneNumber = inv.Invoice.TaxPayer.PhoneNumber,
                            Recipient = inv.Invoice.TaxPayer.Recipient,
                            TIN = inv.Invoice.TaxPayer.TaxPayerIdentificationNumber,
                            AmountDue = inv.Invoice.InvoiceAmountDueSummary.AmountDue,
                            FileNumber = inv.Request.FileRefNumber,
                            InvoiceNumber = inv.Invoice.InvoiceNumber,
                            Address = inv.Invoice.TaxPayer.Address,
                            Transactions = inv.Invoice.Payments.Select(p => new InvoicePaymentsVM
                            {
                                AmountPaid = p.AmountPaid,
                                PaymentReference = p.PaymentReference,
                                ReceiptNumber = p.Receipt.ReceiptNumber,
                                PaymentDate = p.CreatedAtUtc.ToString("dd MMMM yyyy"),
                                TypeID = p.TypeID,
                                InvoiceDesc = p.RevenueHead.Name
                            }).ToList()
                        }).ToList().FirstOrDefault();


                if (invoiceDetails != null && invoiceDetails.Transactions.Any())
                {
                    invoiceDetails.Transactions = invoiceDetails.Transactions.DefaultIfEmpty().Where(p => p.TypeID == (int)PaymentType.Credit && p.ReceiptNumber == receiptNumber).ToList();
                }

                return invoiceDetails;
            }
            catch (Exception exception)
            { Logger.Error(exception, exception.Message); throw; }
        }


        /// <summary>
        /// here we update the service request status
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="oldDefinitionLevelId"></param>
        /// <param name="serviceId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="status"></param>
        public void UpdateServiceRequestsStatus(long requestId, int oldDefinitionLevelId, int serviceId, PSSRequestStatus status, Int64 invoiceId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PoliceServiceRequest).Name;
                string statusName = nameof(PoliceServiceRequest.Status);
                string updatedAtName = nameof(PoliceServiceRequest.UpdatedAtUtc);
                string requestIdName = nameof(PoliceServiceRequest.Request) + "_Id";
                string serviceIdName = nameof(PoliceServiceRequest.Service) + "_Id";
                string flowDefIdName = nameof(PoliceServiceRequest.FlowDefinitionLevel) + "_Id";
                string invoiceIdName = nameof(PoliceServiceRequest.Invoice) + "_Id";

                var queryText = $"UPDATE psr SET psr.{statusName} = :approvedVal, psr.{updatedAtName} = :updateDate FROM {tableName} psr WHERE {requestIdName} = :requestId AND {serviceIdName} = :serviceId AND {flowDefIdName} = :flowDefId AND {invoiceIdName} = :invoiceId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("approvedVal", (int)status);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("requestId", requestId);
                query.SetParameter("serviceId", serviceId);
                query.SetParameter("flowDefId", oldDefinitionLevelId);
                query.SetParameter("invoiceId", invoiceId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Get generic police request document info
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public IEnumerable<GenericRequestDetailsVM> GetGenericDocumentInfo(long requestId)
        {
            return _transactionManager.GetSession().Query<PoliceServiceRequest>()
                .Where(gr => gr.Request.Id == requestId)
                .Select(gr => new GenericRequestDetailsVM
                {
                    TaxEntity = new TaxEntityViewModel
                    {
                        Recipient = gr.Request.TaxEntity.Recipient,
                    },
                    ServiceName = gr.Service.Name,
                    CommandName = gr.Request.Command.Name,
                    CommandStateName = gr.Request.Command.State.Name,
                    CommandLgaName = gr.Request.Command.LGA.Name,
                    RequestDate = gr.Request.CreatedAtUtc,
                    ApprovalDate = gr.Request.UpdatedAtUtc.Value,
                    ApprovalNumber = gr.Request.ApprovalNumber,
                    CbsUser = new CBSUserVM { Name = gr.Request.CBSUser.Name }
                }).ToFuture();
        }


        /// <summary>
        /// Creates a new entry in the police service request table
        /// </summary>
        /// <param name="requestId">current request id</param>
        /// <param name="flowDefinitionLevelId">current flow definition level id</param>
        /// <param name="nextFlowDefinitionLevelId">the flow definition level id of the level the request is being moved to</param>
        public void SavePoliceServiceRequest(long requestId, int flowDefinitionLevelId, int nextFlowDefinitionLevelId)
        {
            if (_transactionManager.GetSession().Query<PoliceServiceRequest>().Count(x => (x.FlowDefinitionLevel.Id == nextFlowDefinitionLevelId) && (x.Request.Id == requestId)) > 0) { return; }
            PSServiceRequestInvoiceValidationDTO requestDetails = _transactionManager.GetSession().Query<PoliceServiceRequest>()
                .Where(x => (x.Request.Id == requestId) && (x.FlowDefinitionLevel.Id == flowDefinitionLevelId))
                .Select(x => new PSServiceRequestInvoiceValidationDTO { RevenueHeadId = x.RevenueHead.Id, InvoiceId = x.Invoice.Id, Request = x.Request, ServiceId = x.Service.Id, ServiceRequestStatus = x.Status })
                .SingleOrDefault();

            string tableName = "Parkway_CBS_Police_Core_" + typeof(PoliceServiceRequest).Name;
            string queryString = $"INSERT INTO {tableName}({nameof(PoliceServiceRequest.RevenueHead)}_Id, {nameof(PoliceServiceRequest.Invoice)}_Id, {nameof(PoliceServiceRequest.Request)}_Id, {nameof(PoliceServiceRequest.Service)}_Id, {nameof(PoliceServiceRequest.Status)}, {nameof(PoliceServiceRequest.CreatedAtUtc)}, {nameof(PoliceServiceRequest.UpdatedAtUtc)}, {nameof(PoliceServiceRequest.FlowDefinitionLevel)}_Id) VALUES(:revenueHeadId, :invoiceId, :requestId, :serviceId, :status, GETDATE(), GETDATE(), :flowDefinitionLevelId)";

            var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
            query.SetParameter("revenueHeadId", requestDetails.RevenueHeadId);
            query.SetParameter("invoiceId", requestDetails.InvoiceId);
            query.SetParameter("requestId", requestId);
            query.SetParameter("serviceId", requestDetails.ServiceId);
            query.SetParameter("status", requestDetails.ServiceRequestStatus);
            query.SetParameter("flowDefinitionLevelId", nextFlowDefinitionLevelId);
            query.ExecuteUpdate();
        }

    }
}