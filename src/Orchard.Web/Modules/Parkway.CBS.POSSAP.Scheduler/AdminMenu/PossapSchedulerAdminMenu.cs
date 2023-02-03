using Orchard.Localization;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.POSSAP.Scheduler.AdminMenu
{
    public class PossapSchedulerAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public PossapSchedulerAdminMenu()
        {
            T = NullLocalizer.Instance;
        }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Scheduling"), "0", PossapSchedulerMenu);
        }


        private void PossapSchedulerMenu(NavigationItemBuilder menu)
        {
            menu.LinkToFirstChild(false)
                .Add(localItem => localItem.Caption(T("Requests Report")).Action(("Report"), "PoliceOfficerSchedulingReport", new { Area = "Parkway.CBS.POSSAP.Scheduler" }))
                .Add(localItem => localItem.Caption(T("Police Officer Report")).Action(("Report"), "PoliceOfficerSchedulerReport", new { Area = "Parkway.CBS.POSSAP.Scheduler" }))
                .Add(localItem => localItem.Caption(T("Deployment Report")).Action(("Report"), "PoliceOfficerDeploymentReport", new { Area = "Parkway.CBS.POSSAP.Scheduler" }));
        }

    }
}