using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class TaxClearanceCertificateManager : BaseManager<TaxClearanceCertificate>, ITaxClearanceCertificateManager<TaxClearanceCertificate>
    {
        private readonly IRepository<TaxClearanceCertificate> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public TaxClearanceCertificateManager(IRepository<TaxClearanceCertificate> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;

        }

        /// <summary>
        /// Gets tax clearance certificate with the specified TCC Number
        /// </summary>
        /// <param name="tccNumber"></param>
        /// <returns></returns>
        public TCCertificateDetailsVM GetCertificateDetails(string tccNumber)
        {
            return _transactionManager.GetSession().Query<TaxClearanceCertificate>().Where(x => x.TCCNumber == tccNumber).Select(x => new TCCertificateDetailsVM
            {
                ApplicantName = x.ApplicantName,
                ResidentialAddress = x.ResidentialAddress,
                OfficeAddress = x.OfficeAddress,
                TCCNumber = x.TCCNumber,
                TaxEntity = x.TaxEntity,
                ApplicationYear = x.ApplicationYear,
                TaxClearanceCertificateRequest = x.TaxClearanceCertificateRequest,
                AddedBy = x.AddedBy,
                TotalIncomeAndTaxAmountPaidWithYear = x.TotalIncomeAndTaxAmountPaidWithYear,
                RevenueOfficerSignatureBlob = x.RevenueOfficerSignatureBlob,
                RevenueOfficerSignatureContentType = x.RevenueOfficerSignatureContentType,
                DirectorOfRevenueSignatureBlob = x.DirectorOfRevenueSignatureBlob,
                DirectorOfRevenueSignatureContentType = x.DirectorOfRevenueSignatureContentType,
                TaxClearanceCertificateTemplate = x.TaxClearanceCertificateTemplate,
                LastDateModified = x.TaxClearanceCertificateRequest.UpdatedAtUtc.Value.ToString("dd MMMM yyyy")
            }).SingleOrDefault();
        }
    }
}