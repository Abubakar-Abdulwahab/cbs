namespace Parkway.CBS.Core.HelperModels
{
    public class TaxEntityCategorySettings
    {
        public bool CanShowDropDown { get; set; }

        public bool IsPhoneNumberRequired { get; set; }

        public bool IsFederalAgency { get; set; }

        public bool ValidateContactEntityInfo { get; set; }

        public bool ValidateGenderInfo { get; set; }

        public bool ShowCorporateRequestReport { get; set; }

        public bool IsEmployer { get; set; }

        public bool CanShowSubUsersRequestReport { get; set; }
    }
}