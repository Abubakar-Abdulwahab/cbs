using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class TaxClearanceCertificateRequestManager : BaseManager<TaxClearanceCertificateRequest>, ITaxClearanceCertificateRequestManager<TaxClearanceCertificateRequest>
    {
        private readonly IRepository<TaxClearanceCertificateRequest> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxClearanceCertificateRequestManager(IRepository<TaxClearanceCertificateRequest> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _transactionManager = orchardServices.TransactionManager;
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Get TCC request details for certificate generation using the specified tcc number
        /// </summary>
        /// <param name="tccNumber"></param>
        /// <returns>TCCertificateVM</returns>
        public TCCertificateVM GetCertificateRequestDetails(string tccNumber)
        {
            return _transactionManager.GetSession().Query<TaxClearanceCertificateRequest>().Where(x => x.TCCNumber == tccNumber)
                 .Select(x => new TCCertificateVM
                 {
                     ApplicantName = x.ApplicantName,
                     Address = x.ResidentialAddress,
                     TCCNumber = x.TCCNumber,
                     YearAppliedFor = x.ApplicationYear,
                     TaxEntityId = x.TaxEntity.Id,
                     LastDateModified = x.UpdatedAtUtc.Value.ToString("dd MMMM yyyy")
                 }).SingleOrDefault();
        }


        /// <summary>
        /// Get TCC request details for TCC request with specified application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns></returns>
        public TCCRequestDetailVM GetRequestDetails(string applicationNumber)
        {
            TaxClearanceCertificateRequest tccRequest = Get(x => x.ApplicationNumber == applicationNumber);

            if (tccRequest == null)
            {
                throw new NoRecordFoundException("404 for TCC application request. Request Application Number " + applicationNumber);
            }
            return new TCCRequestDetailVM
            {
                Id = tccRequest.Id,
                ApplicantName = tccRequest.ApplicantName,
                PhoneNumber = tccRequest.PhoneNumber,
                Status = (TCCRequestStatus)tccRequest.Status,
                ResidentialAddress = tccRequest.ResidentialAddress,
                OfficeAddress = tccRequest.OfficeAddress,
                LandlordName = tccRequest.LandlordName,
                LandlordAddress = tccRequest.LandlordAddress,
                RequestDate = tccRequest.CreatedAtUtc.ToString("dd/MM/yyyy"),
                Occupation = tccRequest.Occupation,
                TIN = tccRequest.TIN,
                IsRentedApartment = tccRequest.IsRentedApartment,
                RequestReason = tccRequest.RequestReason,
                ExemptionType = (TCCExemptionType)tccRequest.ExemptionCategory,
                ExemptionTypeS = ((TCCExemptionType)tccRequest.ExemptionCategory).ToDescription(),
                ApplicationNumber = tccRequest.ApplicationNumber,
                DevelopmentLevyInvoiceNumber = tccRequest.DevelopmentLevyInvoice.InvoiceNumber,
                HusbandName = tccRequest.HusbandName,
                HusbandAddress = tccRequest.HusbandAddress,
                IdentificationNumber = tccRequest.IdentificationNumber,
                InstitutionName = tccRequest.InstitutionName,
                TCCNumber = tccRequest.TCCNumber,
                ApplicationYear = tccRequest.ApplicationYear,
                TaxEntityId = tccRequest.TaxEntity.Id,
                ApprovalStatusLevelId = tccRequest.ApprovalStatusLevelId,
                Attachments = tccRequest.FileUploads.Select(x => new TCCRequestAttachmentVM()
                {
                    UploadTypeName = ((TCCFileUploadType)x.TCCFileUploadTypeId).ToDescription(),
                    FilePath = x.FilePath,
                    ContentType = x.ContentType
                }).ToList()
            };
        }

        /// <summary>
        /// Get TCC request id using the specified application number
        /// </summary>
        /// <param name="applicationNumber"></param>
        /// <returns>Int64</returns>
        public long GetRequestRequestId(string applicationNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<TaxClearanceCertificateRequest>().Where(x => x.ApplicationNumber == applicationNumber)
                    .Select(x => x.Id).Single();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new NoRecordFoundException($"Request with application number {applicationNumber} not found");
            }
        }
    }
}