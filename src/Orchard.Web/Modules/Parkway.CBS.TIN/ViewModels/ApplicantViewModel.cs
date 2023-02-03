using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.TIN.ViewModels
{
    public class ApplicantViewModel
    {
        public string Title { get; set; }
        public string ApplicantName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }
        public Core.Models.Enums.Gender Sex { get; set; }
        //public Enumerations.ApplicantType ApplicantType { get; set; }
        public string IdentificationNumber { get; set; }
        public string IdentificationFilePath { get; set; }
        public Core.Models.Enums.IdentificationType IdentificationType { get; set; }
        public string Nationality { get; set; }
        public string StateOfOrigin { get; set; }
        public string LGA { get; set; }
        public string Occupation { get; set; }
        public string TIN { get; set; }
        public string CompanyName { get; set; }
        public string RCNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfRegistration { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public Core.Models.Enums.MaritalStatus MaritalStatus { get; set; }
        public string MotherMaidenName { get; set; }
        public string MotherName { get; set; }
        public IEnumerable<TaxEntityCategory> TaxEntityCategories { get; set; }
        public int TaxEntityCategoryId { get; set; }
        public dynamic Pager { get; set; }
        public IList<dynamic> TableData { get; set; }
    }
}