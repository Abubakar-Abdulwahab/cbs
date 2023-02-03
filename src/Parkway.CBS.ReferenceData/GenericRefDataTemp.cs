namespace Parkway.CBS.ReferenceData
{
    public class GenericRefDataTemp
    {
        public virtual string Recipient { get; set; }

        public virtual string Address { get; set; }

        public virtual int TaxEntityCategoryId { get; set; }

        public virtual int BillingModelId { get; set; }

        public virtual int RevenueHeadId { get; set; }

        public virtual string TaxIdentificationNumber { get; set; }

        public virtual string Email { get; set; }

        public virtual string BatchNumber { get; set; }

        public virtual int Status { get; set; }

        public virtual string AdditionalDetails { get; set; }

        public virtual string ErrorLog { get; set; }

        public virtual string StatusDetail { get; set; }

        public virtual decimal Amount { get; set; }

        /// <summary>
        /// Concat of tax identification number
        /// </summary>
        public virtual string UniqueIdentifier { get; set; }
    }
}