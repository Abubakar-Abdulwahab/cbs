using Orchard.Localization;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.AdminMenu
{
    public class OSGOFAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public OSGOFAdminMenu()
        {
            T = NullLocalizer.Instance;
        }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {

            builder.Add(T("Operator"), "0", CellOperatorMenu);
        }

        private void CellOperatorMenu(NavigationItemBuilder menu)
        {
            menu.Add(item => item.LinkToFirstChild(false))
                .Add(localItem => localItem.Caption(T("Add Operator")).Action(("AddOperator"), "Operator", new { Area = "Parkway.CBS.OSGOF.Admin" }))
                .Add(localItem => localItem.Caption(T("Add Cell Sites")).Action(("SearchOperator"), "Operator", new { Area = "Parkway.CBS.OSGOF.Admin" }))
                .Add(localItem => localItem.Caption(T("Cell Sites")).Action(("List"), "CellSites", new { Area = "Parkway.CBS.OSGOF.Admin" }));
        }


    }
}