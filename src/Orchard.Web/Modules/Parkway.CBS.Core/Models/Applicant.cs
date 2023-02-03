using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class Applicant : CBSBaseModel
    {
        public virtual long Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Phone2 { get; set; }
        public virtual string Phone3 { get; set; }
        public virtual Enums.Gender Sex { get; set; }
        public virtual TaxEntityCategory TaxEntityCategory { get; set; }
        public virtual string IdentificationNumber { get; set; }
        public virtual string IdentificationFilePath { get; set; }
        public virtual Enums.IdentificationType IdentificationType { get; set; }
        public virtual string Nationality { get; set; }
        public virtual string StateOfOrigin { get; set; }
        public virtual string LGA { get; set; }
        public virtual string Occupation { get; set; }
        public virtual string TIN { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string RCNumber { get; set; }
        //public virtual string CACCertificate { get; set; }
        //public virtual string TaxCertificate { get; set; }
        //public virtual string IndividualTaxCertificate { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual DateTime? DateOfRegistration { get; set; }
        public virtual DateTime? DateOfIncorporation { get; set; }
        public virtual Enums.MaritalStatus MaritalStatus { get; set; }
        public virtual string MotherMaidenName { get; set; }
        public virtual string MotherName { get; set; }

    }
}