using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class ValidateIdentificationNumberResponseModel
    {
        public int IdType { get; set; }

        public bool IsActive { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public string EmailAddress { get; set; }

        public string TaxPayerName { get; set; }

        public string PhoneNumber { get; set; }

        public string RCNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
    }
}