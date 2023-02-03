using Orchard.Localization;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.AdminMenu
{
    public class DataEnumerationAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public DataEnumerationAdminMenu()
        {
            T = NullLocalizer.Instance;
        }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Enumeration Data"), "0", DataEnumerationMenu);
        }

        private void DataEnumerationMenu(NavigationItemBuilder menu)
        {
            menu.Add(item => item.LinkToFirstChild(false))
                .Add(localItem => localItem.Caption(T("Upload data")).Action(("UploadData"), "Enumeration", new { Area = "Parkway.CBS.ReferenceData.Admin" }))
                .Add(localItem => localItem.Caption(T("Reference Data Status")).Action(("CheckBatchRecords"), "Enumeration", new { Area = "Parkway.CBS.ReferenceData.Admin" }))
                .Add(localItem => localItem.Caption(T("NAGIS Data Status")).Action(("CheckNAGISBatchRecords"), "Enumeration", new { Area = "Parkway.CBS.ReferenceData.Admin" }));
        }

    }
}