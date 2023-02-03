using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class HeaderObj
    {
        public bool IsLoggedIn { get; set; }

        public bool ShowSignin { get; set; }

        public string DisplayText { get; set; }

        public string MinistryLogo { get; set; }

        public int CategoryId { get; set; }

        public bool DontShowLinks { get; set; }

        public TaxEntityCategorySettings TaxEntityCategorySettings { get; set; }

        public bool IsAdministrator { get; set; }
    }
}