using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Parkway.CBS.TIN
{
    public class TINAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public TINAdminMenu()
        {
            T = NullLocalizer.Instance;
        }

        public string MenuName
        {
            get { return "admin"; }
        }


        public void GetNavigation(NavigationBuilder builder)
        {
            
        }
    }
}