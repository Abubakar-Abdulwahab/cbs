using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class RegisterCBSUserObj
    {

        public HeaderObj HeaderObj { get; set; }

        public string ErrorMessage { get; set; }

        public bool Error { get; set; }

        public RegisterCBSUserModel RegisterCBSUserModel { get; set; }

        public List<TaxEntityCategory> TaxCategories { get; set; }

        public List<TaxEntityCategoryVM> TaxCategoriesVM { get; set; }

        public List<TaxCategoryTaxCategoryPermissionsVM> TaxCategoryPermissions { get; set; }

        public string TaxPayerType { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }

        public FlashObj FlashObj { get; set; }

        public bool ShowModal { get; set; }

        public string PSSServiceName { get; set; }

        public string PSSServiceNote { get; set; }
    }
}