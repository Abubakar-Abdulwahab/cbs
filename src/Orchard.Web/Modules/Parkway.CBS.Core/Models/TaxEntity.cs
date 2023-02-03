using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class TaxEntity : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        [StringLength(250, ErrorMessage = "The Recipient can only be 250 characters long or 5 characters short", MinimumLength = 5)]
        public virtual string Recipient { get; set; }

        [StringLength(255, ErrorMessage = "The Email value can only be 255 characters long or 2 characters short", MinimumLength = 2)]
        [EmailAddress(ErrorMessage = "Provide an valid Email value")]
        public virtual string Email { get; set; }

        [StringLength(500, ErrorMessage = "The Address value can only be 500 characters long or 5 characters short", MinimumLength = 5)]
        public virtual string Address { get; set; }

        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// this is the taxcategory identifier, (can't remember why I have it here)
        /// </summary>
        public virtual int TaxEntityType { get; set; }

        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        [StringLength(100, ErrorMessage = "The Tax Payer Identification Number value can only be 100 characters long")]
        public virtual string TaxPayerIdentificationNumber { get; set; }

        public virtual long PrimaryContactId { get; set; }

        public virtual long CashflowCustomerId { get; set; }

        public virtual string RCNumber { get; set; }

        public virtual string ContactPersonName { get; set; }

        public virtual string ContactPersonEmail { get; set; }

        public virtual string ContactPersonPhoneNumber { get; set; }

        public virtual string ShortName { get; set; }

        public virtual TaxEntityAccount TaxEntityAccount { get; set; }

        public virtual IEnumerable<TransactionLog> TransactionLog { get; set; }

        //additional props from nasarawa scrap file
        #region props for scrap file

        public string PreviousTIN { get; set; }

        public DateTime? DOB { get; set; }

        public Gender Gender { get; set; }

        public DateTime? RegDate { get; set; }

        public string LGA { get; set; }

        public string Ward { get; set; }

        public string Occupation { get; set; }

        public string SourceOfIncome { get; set; }

        public string Nationality { get; set; }

        public string State { get; set; }

        public string StateOfOrigin { get; set; }

        public string TaxAuthority { get; set; }

        #endregion

        /// <summary>
        /// For every tax profile that has been created a corresponding payer id is created
        /// </summary>
        public virtual string PayerId { get; set; }

        /// <summary>
        /// Tax payer code
        /// </summary>
        public virtual string TaxPayerCode { get; set; }

        /// <summary>
        /// If the tax profile has been disabled
        /// </summary>
        public virtual bool IsDisabled { get; set; }

        /// <summary>
        /// use this prop to flag profiles that are unknown
        /// </summary>
        public virtual bool UnknownProfile { get; set; }


        public virtual ExpertSystemSettings AddedByExternalExpertSystem { get; set; }

        public virtual LGA StateLGA { get; set; }

        public virtual string BVN { get; set; }

        public virtual int BusinessType { get; set; }

        public virtual int IdentificationType { get; set; }

        public virtual string IdentificationNumber { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string MiddleName { get; set; }

        public virtual string LastName { get; set; }

        public override string ToString()
        {
            return string.Format("Email - {0}, TIN - {1}, PhoneNumber - {2} ", Email,
                TaxPayerIdentificationNumber, PhoneNumber);
        }
    }
}