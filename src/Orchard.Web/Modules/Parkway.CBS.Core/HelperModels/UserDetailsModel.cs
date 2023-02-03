using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.HelperModels
{
    public class UserDetailsModel
    {
        public TaxEntity Entity { get; set; }

        public string Name { get; set; }

        public CBSUser CBSUser { get; set; }

        public TaxEntityCategory Category { get; set; }

        public TaxEntityCategoryVM CategoryVM { get; set; }

        public TaxEntityViewModel TaxPayerProfileVM { get; set; }

        public CBSUserVM CBSUserVM { get; set; }
    }

    public class ModelForTokenRegeneration
    {
        public CBSUserVM CBSUserVM { get; set; }

        public TaxEntityViewModel TaxPayerProfileVM { get; set; }
    }

}