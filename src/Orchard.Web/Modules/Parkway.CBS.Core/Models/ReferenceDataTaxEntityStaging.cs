using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class ReferenceDataTaxEntityStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual ReferenceDataBatch ReferenceDataBatch { get; set; }

        public virtual ReferenceDataRecords ReferenceDataRecords { get; set; }

        public virtual string PhoneNumber { get; set; }

        // This is use to determine if the Payer details will be created in TaxEntity table
        // or updated if already exist
        public virtual int OperationType { get; set; }

        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        /// <summary>
        /// This is use to track if tax payer is the landlord
        /// If false, then, the tax payer and landlord details will be created separately in the entity table
        /// </summary>
        public virtual Boolean IsTaxPayerLandlord { get; set; }

        public virtual decimal PropertyRentAmount { get; set; }

        public virtual Boolean IsEvidenceProvided { get; set; }

        public virtual ReferenceDataOperationType GetOperationType()
        {
            return (ReferenceDataOperationType)this.OperationType;
        }

        public virtual string Surname { get; set; }

        public virtual string Firstname { get; set; }

        public virtual string Middlename { get; set; }

        public virtual string TIN { get; set; }

        public virtual string HouseNo { get; set; }

        public virtual string StreetName { get; set; }

        public virtual string City { get; set; }

        public virtual string DbLGAId { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual string TaxEntityCategoryId { get; set; }

    }
}