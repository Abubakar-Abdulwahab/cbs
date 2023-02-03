using Orchard.Localization;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ETCC.Admin.AdminMenu
{
    public class ETCCAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public ETCCAdminMenu()
        {
            T = NullLocalizer.Instance;
        }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("TCC Request"), "0", ETCCMenu);
        }


        private void ETCCMenu(NavigationItemBuilder menu)
        {
            menu.Add(item => item.LinkToFirstChild(false))
                .Add(localItem => localItem.Caption(T("Requests Report")).Action(("RequestReport"), "TCCReport", new { Area = "Parkway.CBS.ETCC.Admin" }))
                .Add(localItem => localItem.Caption(T("Requests Approval")).Action(("TCCRequests"), "RequestApproval", new { Area = "Parkway.CBS.ETCC.Admin" }))
                .Add(localItem => localItem.Caption(T("Receipt Utilizations")).Action(("Receipts"), "ReceiptUtilizationReport", new { Area = "Parkway.CBS.ETCC.Admin" }).Permission(Permissions.CanViewReceiptUtilizations))
                .Add(localItem => localItem.Caption(T("Direct Assessment Report")).Action(("DirectAssessmentRequestReport"), "DirectAssessmentReport", new { Area = "Parkway.CBS.ETCC.Admin" }).Permission(Permissions.ViewDirectAssessmentReport));

        }

    }
}