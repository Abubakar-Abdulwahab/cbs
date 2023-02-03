using Orchard.Users.Models;
using System;

namespace Parkway.CBS.Core.Models
{
    public class TaxClearanceCertificate : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string ApplicantName { get; set; }

        public virtual string ResidentialAddress { get; set; }

        public virtual string OfficeAddress { get; set; }

        public virtual string TCCNumber { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual int ApplicationYear { get; set; }

        public virtual TaxClearanceCertificateRequest TaxClearanceCertificateRequest { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }
        /// <summary>
        /// Json Array containing the total income and tax amount paid for each of the three years for which the certificate was generated for.
        /// </summary>
        public virtual string TotalIncomeAndTaxAmountPaidWithYear { get; set; }

        public virtual TaxClearanceCertificateAuthorizedSignatures RevenueOfficerSignature { get; set; }

        public virtual TaxClearanceCertificateAuthorizedSignatures DirectorOfRevenueSignature { get; set; }

        /// <summary>
        /// Base 64 string of the signature
        /// </summary>
        public virtual string RevenueOfficerSignatureBlob { get; set; }

        public virtual string RevenueOfficerSignatureContentType { get; set; }

        /// <summary>
        /// Base 64 string of the signature
        /// </summary>
        public virtual string DirectorOfRevenueSignatureBlob { get; set; }

        public virtual string DirectorOfRevenueSignatureContentType { get; set; }

        public virtual string TaxClearanceCertificateTemplate { get; set; }
    }
}