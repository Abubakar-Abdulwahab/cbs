using Newtonsoft.Json;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class TCCertificateDetailsVM
    {
        public Int64 Id { get; set; }

        public string ApplicantName { get; set; }

        public string ResidentialAddress { get; set; }

        public string OfficeAddress { get; set; }

        public string TCCNumber { get; set; }

        public TaxEntity TaxEntity { get; set; }

        public int ApplicationYear { get; set; }

        public TaxClearanceCertificateRequest TaxClearanceCertificateRequest { get; set; }

        public UserPartRecord AddedBy { get; set; }
        /// <summary>
        /// Json Array containing the total income and tax amount paid for each of the three years for which the certificate was generated for.
        /// </summary>
        public string TotalIncomeAndTaxAmountPaidWithYear { get; set; }

        /// <summary>
        /// TotalIncomeAndTaxAmountPaidWithYear Deserialized
        /// </summary>
        public IEnumerable<TCCYearlyTaxSummary> TotalIncomeAndTaxAmountPaidWithYearCollection => JsonConvert.DeserializeObject<IEnumerable<TCCYearlyTaxSummary>>(TotalIncomeAndTaxAmountPaidWithYear);

        public string RevenueOfficerSignatureBlob { get; set; }

        public string RevenueOfficerSignatureContentType { get; set; }

        public string DirectorOfRevenueSignatureBlob { get; set; }

        public string DirectorOfRevenueSignatureContentType { get; set; }

        public string TaxClearanceCertificateTemplate { get; set; }

        public string LastDateModified { get; set; }
    }
}