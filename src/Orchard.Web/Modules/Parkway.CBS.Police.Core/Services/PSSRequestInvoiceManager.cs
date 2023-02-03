using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSRequestInvoiceManager : BaseManager<PSSRequestInvoice>, IPSSRequestInvoiceManager<PSSRequestInvoice>
    {
        private readonly IRepository<PSSRequestInvoice> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public PSSRequestInvoiceManager(IRepository<PSSRequestInvoice> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }


        ///// <summary>
        ///// Get invoices for request with specified File ref number
        ///// </summary>
        ///// <param name="fileNumber"></param>
        ///// <returns>List<RequestInvoiceVM></returns>
        //public ICollection<RequestInvoiceVM> GetRequestInvoices(string fileNumber)
        //{
        //    return _transactionManager.GetSession().Query<PSSRequestInvoice>()
        //    .Where(sr => sr.Request.FileRefNumber == fileNumber)
        //    .Select(requestInvoice => new RequestInvoiceVM
        //    {
        //        Id = requestInvoice.Id,
        //        InvoiceNumber = requestInvoice.Invoice.InvoiceNumber,
        //        InvoiceUrl = requestInvoice.Invoice.InvoiceURL,
        //        ServiceName = requestInvoice.Request.Service.Name,
        //        FileRefNumber = requestInvoice.Request.FileRefNumber,
        //        Status = requestInvoice.Invoice.Status,
        //        Amount = requestInvoice.Invoice.Amount,
        //        AmountDue = requestInvoice.Invoice.InvoiceAmountDueSummary.AmountDue
        //    }).ToList();
        //}


        ///// <summary>
        ///// Get invoices for request with specified Id
        ///// </summary>
        ///// <param name="requestId"></param>
        ///// <returns>List<RequestInvoiceVM></returns>
        //public ICollection<RequestInvoiceVM> GetRequestInvoices(long requestId)
        //{
        //    return _transactionManager.GetSession().Query<PSSRequestInvoice>()
        //    .Where(sr => sr.Request == new PSSRequest { Id = requestId })
        //    .Select(requestInvoice => new RequestInvoiceVM
        //    {
        //        Id = requestInvoice.Id,
        //        InvoiceNumber = requestInvoice.Invoice.InvoiceNumber,
        //        InvoiceUrl = requestInvoice.Invoice.InvoiceURL,
        //        ServiceName = requestInvoice.Request.Service.Name,
        //        FileRefNumber = requestInvoice.Request.FileRefNumber,
        //        Status = requestInvoice.Invoice.Status,
        //        Amount = requestInvoice.Invoice.Amount,
        //        AmountDue = requestInvoice.Invoice.InvoiceAmountDueSummary.AmountDue
        //    }).ToList();
        //}


        /// <summary>
        /// Gets CBSUserVM for invoice with specified invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public CBSUserVM GetCBSUserWithInvoiceNumber(string invoiceNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSRequestInvoice>()
                    .Where(x => x.Invoice.InvoiceNumber == invoiceNumber)
                    .Select(x => new CBSUserVM { Id = x.Request.CBSUser.Id, Name = x.Request.CBSUser.Name, PhoneNumber = x.Request.CBSUser.PhoneNumber, Email = x.Request.CBSUser.Email, IsAdministrator = x.Request.CBSUser.IsAdministrator, TaxEntity = new TaxEntityViewModel { Id = x.Request.CBSUser.TaxEntity.Id } }).Single();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get PSSRequestInvoice with specified invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public PSSRequestInvoiceDTO GetPSSRequestInvoiceWithInvoiceNumber(string invoiceNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSSRequestInvoice>().Where(x => x.Invoice.InvoiceNumber == invoiceNumber).Select(x => new PSSRequestInvoiceDTO
                {
                    Id = x.Id,
                    InvoiceStatus = x.Invoice.Status,
                    RequestId = x.Request.Id,
                    InvoiceId = x.Invoice.Id
                }).SingleOrDefault();
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Gets request invoice details with specified request id and invoice number
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="invoiceNumber"></param>
        /// <returns>IEnumerable{PSServiceRequestInvoiceValidationDTO}</returns>
        public IEnumerable<PSServiceRequestInvoiceValidationDTO> GetRequestInvoiceDetailsWithRequestIdAndInvoiceNumber(long requestId, string invoiceNumber)
        {
            return _transactionManager.GetSession().Query<PSSRequestInvoice>()
                .Where(sr => (sr.Request.Id == requestId) && (sr.Invoice.InvoiceNumber == invoiceNumber))
                .Select(sr => new PSServiceRequestInvoiceValidationDTO
                {
                    AmountDue = sr.Invoice.InvoiceAmountDueSummary.AmountDue,
                    InvoiceStatus = (InvoiceStatus)sr.Invoice.Status,
                    Request = new PSSRequest { Id = sr.Request.Id, ApprovalNumber = sr.Request.ApprovalNumber, FileRefNumber = sr.Request.FileRefNumber },
                    PaymentDate = sr.Invoice.PaymentDate,
                    CancellationDate = sr.Invoice.CancelDate,
                    InvoiceNumber = sr.Invoice.InvoiceNumber,
                    InvoiceId = sr.Invoice.Id,
                    ServiceType = (Models.Enums.PSSServiceTypeDefinition)sr.Request.Service.ServiceType,
                    TaxEntityId = sr.Request.TaxEntity.Id,
                    PhoneNumber = string.IsNullOrEmpty(sr.Request.ContactPersonPhoneNumber) ? sr.Request.CBSUser.PhoneNumber : sr.Request.ContactPersonPhoneNumber,
                    Recipient = sr.Request.CBSUser.Name,
                    Email = string.IsNullOrEmpty(sr.Request.ContactPersonEmail) ? sr.Request.CBSUser.Email : sr.Request.ContactPersonEmail,
                    ServiceName = sr.Request.Service.Name,
                });
        }
    }
}