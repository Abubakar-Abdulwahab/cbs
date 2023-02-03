using Orchard.Localization;
using Orchard.UI.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.AdminMenu
{
    public class CBSAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public CBSAdminMenu()
        {
            T = NullLocalizer.Instance;
        }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("MDA"), "0", MdaMenu);
            builder.Add(T("Revenue Head"), "1", RevHeadMenu);
            builder.Add(T("Report"), "0", ReportMenu);
            builder.Add(T("Make Payment"), "0", AddPaymentMenu);
            builder.Add(T("Tenant Settings"), "0", SettingsMenu);
            builder.Add(T("Invoice Settings"), "0", GenerateInvoiceMenu);
            builder.Add(T("Payment Provider"), "0", AddExternalPaymentProviderMenu);
            //builder.AddImageSet("users").Add(T("Admin Users"), "0", UserManagementMenu);
        }

        private void SettingsMenu(NavigationItemBuilder menu)
        {
            menu.Permission(Permissions.TenantSettingMainMenu)
                .Add(item => item.LinkToFirstChild(false))
                .Add(localItem => localItem.Caption(T("Change Password")).Action(("ChangePassword"), "Settings", new { Area = "Parkway.CBS.Module" }))
                .Add(localItem => localItem.Caption(T("Admin Settings")).Action(("ListOfExpertSystems"), "Settings", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.ManageAdminSettings))
                .Add(localItem => localItem.Caption(T("Create Settlement")).Action(("CreateSettlementRule"), "Settlement", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.CreateSettlementRule))
                .Add(localItem => localItem.Caption(T("Settlements")).Action(("Settlements"), "Settlement", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.CreateSettlementRule));
            //.Add(localItem => localItem.Caption(T("Reference Data Settings")).Action(("ReferenceDataSettings"), "Settings", new { Area = "Parkway.CBS.Module" }));
        }

        private void GenerateInvoiceMenu(NavigationItemBuilder menu)
        {
            menu.Permission(Permissions.InvoiceSettingMainMenu)
                .Add(item => item.LinkToFirstChild(false))
                .Add(localItem => localItem.Caption(T("Generate Invoice")).Action(("GenerateInvoice"), "Invoice", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.GenerateInvoice))
                .Add(localItem => localItem.Caption(T("Search for Payment References")).Action(("SearchInvoiceNumberForPaymentRef"), "Invoice", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.SearchPaymentReference));
        }

        private void ReportMenu(NavigationItemBuilder menu)
        {
            // menu.Add(localItem => localItem.Caption(T("Reports")).Action(("List"), "MDA", new { Area = "Parkway.CBS.Module" }));
            menu.Permission(Permissions.ReportMainMenu)
                .Add(item => item.LinkToFirstChild(false))
                .Add(localItem => localItem.Caption(T("Invoice Report")).Action(("AssessmentReport"), "Report", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.ViewInvoiceReport))
                .Add(localItem => localItem.Caption(T("TIN Registration Report")).Action(("Index"), "TINReport", new { Area = "Parkway.CBS.TIN" }).Permission(Permissions.ViewTINReport))
                .Add(localItem => localItem.Caption(T("Collection Report")).Action(("CollectionReport"), "Report", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.ViewCollectionReport))
                .Add(localItem => localItem.Caption(T("Tax Payer Report")).Action(("TaxProfilesReport"), "Report", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.ViewTaxPayersReport));
            //.Add(localItem => localItem.Caption(T("Tax Payer Report")).Action(("TaxPayerReport"), "Report", new { Area = "Parkway.CBS.Module" }));
            //.Add(localItem => localItem.Caption(T("Demand Notices")).Action(("DemandNotice"), "Report", new { Area = "Parkway.CBS.Module" }));
            //.Add(localItem => localItem.Caption(T("Tax Payer Statement of Account")).Action(("TaxEntity"), "Report", new { Area = "Parkway.CBS.Module" }));
        }

        private void MdaMenu(NavigationItemBuilder menu)
        {
            //menu.Add(item => item.LinkToFirstChild(false))
            //    .Add(localItem => localItem.Caption(T("MDA")).Action(("List"), "MDA", new { Area = "Parkway.CBS.Module" }))
            //    .Add(localItem => localItem.Caption(T("MDA on Cashflow")).Action(("CreateMDASettings"), "MDA", new { Area = "Parkway.CBS.Module" }));
            menu.Add(localItem => localItem.Caption(T("MDA")).Action(("List"), "MDA", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.ViewMDAList));
        }

        private void AddPaymentMenu(NavigationItemBuilder menu)
        {
            //menu.Add(item => item.LinkToFirstChild(false))
            //    .Add(localItem => localItem.Caption(T("MDA")).Action(("List"), "MDA", new { Area = "Parkway.CBS.Module" }))
            //    .Add(localItem => localItem.Caption(T("MDA on Cashflow")).Action(("CreateMDASettings"), "MDA", new { Area = "Parkway.CBS.Module" }));
            menu.Add(localItem => localItem.Caption(T("AddInvoicePayment")).Action(("SearchInvoicePayment"), "Payment", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.AddPayments));
        }

        private void RevHeadMenu(NavigationItemBuilder menu)
        {
            menu.Permission(Permissions.RevenueHeadMainMenu)
                .Add(item => item.LinkToFirstChild(false))
                .Add(localItem => localItem.Caption(T("Revenue Head")).Action(("List"), "RevenueHead", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.ViewRevenueHeadList));
            //.Add(localItem => localItem.Caption(T("Assessment List")).Action(("BillablesList"), "RevenueHead", new { Area = "Parkway.CBS.Module" }));
            //menu.Add(localItem => localItem.Caption(T("Revenue Head")).Action(("List"), "RevenueHead", new { Area = "Parkway.CBS.Module" }));
        }

        private void AddExternalPaymentProviderMenu(NavigationItemBuilder menu)
        {
            menu.Permission(Permissions.PaymentProviderMainMenu)
                .Add(item => item.LinkToFirstChild(false))
                .Add(localItem => localItem.Caption(T("Create")).Action(("Create"), "ExternalPaymentProvider", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.CreateExternalPaymentProvider))
                .Add(localItem => localItem.Caption(T("Payment Providers")).Action(("List"), "ExternalPaymentProvider", new { Area = "Parkway.CBS.Module" }).Permission(Permissions.ViewExternalPaymentProvider));
        }
    }
}