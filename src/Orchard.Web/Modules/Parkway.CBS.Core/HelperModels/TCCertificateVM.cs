namespace Parkway.CBS.Core.HelperModels
{
    public class TCCertificateVM
    {
        public string ApplicantName { get; set; }

        public string Address { get; set; }

        public decimal YearOneTotalIncome { get; set; }

        public decimal YearOneTaxPaid { get; set; }

        public decimal YearTwoTotalIncome { get; set; }

        public decimal YearTwoTaxPaid { get; set; }

        /// <summary>
        /// TCC year that user applied for total income
        /// </summary>
        public decimal YearAppliedForTotalIncome { get; set; }

        /// <summary>
        /// TCC year that user applied for tax paid
        /// </summary>
        public decimal YearAppliedForTaxPaid { get; set; }

        public int YearOne { get; set; }

        public int YearTwo { get; set; }

        /// <summary>
        /// TCC year that user applied for
        /// </summary>
        public int YearAppliedFor { get; set; }

        public string TCCNumber { get; set; }

        public string ExpireDate { get; set; }

        public string LastDateModified { get; set; }

        public long TaxEntityId { get; set; }

        public string BarCodeSavingPath { get; set; }

        public string LogoURL { get; set; }

        public string LogoURLPath { get; set; }

        public string TCCCertificateBGPath { get; set; }

        public string RevenueOfficerSignature { get; set; }

        public string DirectorOfRevenueSignature { get; set; }
    }
}