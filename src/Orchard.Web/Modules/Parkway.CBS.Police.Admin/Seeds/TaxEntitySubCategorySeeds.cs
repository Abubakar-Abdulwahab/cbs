using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Admin.Seeds.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Admin.Seeds
{
    public class TaxEntitySubCategorySeeds : ITaxEntitySubCategorySeeds
    {
        private readonly ITaxEntitySubCategoryManager<TaxEntitySubCategory> _subCategory;
        private readonly ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> _subSubCategory;

        public TaxEntitySubCategorySeeds(ITaxEntitySubCategoryManager<TaxEntitySubCategory> subCategory, ITaxEntitySubSubCategoryManager<TaxEntitySubSubCategory> subSubCategory)
        {
            _subCategory = subCategory;
            _subSubCategory = subSubCategory;
        }

        public void AddTaxEntitySubCategory()
        {
            List<TaxEntitySubCategory> ranks = new List<TaxEntitySubCategory>
            {
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 1}, Name = "Private", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 1}, Name = "Politically Exposed Persons (PEP)", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Health", IsActive = true} },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Finance", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Oil & Gas", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Agriculture", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "NGO", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Sports", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Information & Communication Technology", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Manufacturing", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Transport", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Education", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Mining", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Aviation", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Telecommunications", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Power & Energy", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Automotive", IsActive = true } },
                { new TaxEntitySubCategory { TaxEntityCategory = new TaxEntityCategory{ Id = 2}, Name = "Others", IsActive = true } },
            };

            if (!_subCategory.SaveBundle(ranks)) { throw new Exception { }; }
        }

        public void AddTaxEntitySubSubCategory()
        {
            List<TaxEntitySubSubCategory> ranks = new List<TaxEntitySubSubCategory>
            {
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 2}, Name = "Federal Government Principal Officer", IsActive = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 2}, Name = "State Government Principal Officer", IsActive = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 2}, Name = "Federal Judiciary", IsActive = true} },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 2}, Name = "State Judiciary", IsActive = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 2}, Name = "Federal Legislature", IsActive = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 2}, Name = "State Legislature", IsActive = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 2}, Name = "Diplomat", IsActive = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 2}, Name = "Traditional Ruler", IsActive = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 2}, Name = "Others", IsActive = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 1}, Name = "Private", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 3}, Name = "Health", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 4}, Name = "Finance", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 5}, Name = "Oil & Gas", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 6}, Name = "Agriculture", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 7}, Name = "NGO", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 8}, Name = "Sports", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 9}, Name = "Information & Communication Technology", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 10}, Name = "Manufacturing", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 11}, Name = "Transport", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 12}, Name = "Education", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 13}, Name = "Mining", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 14}, Name = "Aviation", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 15}, Name = "Telecommunications", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 16}, Name = "Power & Energy", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 17}, Name = "Automotive", IsActive = true, IsDefault = true } },
                { new TaxEntitySubSubCategory { TaxEntitySubCategory = new TaxEntitySubCategory{ Id = 18}, Name = "Corporate Others", IsActive = true, IsDefault = true } },
            };

            if (!_subSubCategory.SaveBundle(ranks)) { throw new Exception { }; }
        }
    }
}