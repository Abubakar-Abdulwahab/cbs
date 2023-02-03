using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Parkway.CBS.Police.Admin.AdminMenu
{
    public class PoliceAdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public PoliceAdminMenu()
        {
            T = NullLocalizer.Instance;
        }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("View Analytics Dashboard"), "0", AnalyticsDashboardMenu);
            builder.Add(T("Police Request"), "0", PoliceReportMenu);
            builder.Add(T("User Management"), "1", PoliceUserManagementMenu);
            builder.Add(T("Signature Upload"), "1", PoliceAdminSignatureUploadMenu);
            builder.Add(T("Command Payment Configuration"), "1", PolicePaymentMenu);
            builder.Add(T("Escort & Guards Regularization"), "1", PoliceEGSRegularizationMenu);
        }


        private void PoliceReportMenu(NavigationItemBuilder menu)
        {
            menu.Permission(Permissions.PoliceRequestMainMenu)
                .LinkToFirstChild(false)
                .Add(localItem => localItem.Caption(T("Collection Report")).Action(("CollectionReport"), "PSSCollectionReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewCollectionReports))
                .Add(localItem => localItem.Caption(T("Police Settlements")).Action(("Report"), "SettlementReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewSettlementReportSummary))
                .Add(localItem => localItem.Caption(T("Police Settlement Report")).Action(("ReportAggregate"), "PSSSettlementReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewSettlementReportSummary))
                .Add(localItem => localItem.Caption(T("Police Settlement Fee Parties")).Action(("FeeParties"), "SettlementFeeParties", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewSettlementFeeParties))
                .Add(localItem => localItem.Caption(T("Police Settlement Report Breakdown")).Action(("ReportBreakdown"), "PSSSettlementReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewSettlementReportSummary))
                .Add(localItem => localItem.Caption(T("Officers Deployment Report")).Action(("PSSDeployedOfficers"), "PSSDeployedOfficersReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewDeployments))
                //.Add(localItem => localItem.Caption(T("Police Officers Report")).Action(("PoliceOfficerReport"), "PoliceOfficerReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewOfficers))
                .Add(localItem => localItem.Caption(T("Officers Deployment Allowance Report")).Action(("PSSDeploymentAllowanceReport"), "PSSDeploymentAllowanceReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewDeploymentAllowanceReports))
                .Add(localItem => localItem.Caption(T("Command Wallet Report")).Action(("CommandWalletReport"), "CommandWalletReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewCommandWalletReports))
                .Add(localItem => localItem.Caption(T("Command Wallet")).Action(("AddCommandWallet"), "CommandWallet", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanCreateCommandWallet))
                .Add(localItem => localItem.Caption(T("Officers Deployment Allowance Request")).Action(("DeploymentAllowanceRequest"), "PSSDeploymentAllowanceRequest", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewDeploymentAllowanceRequests))
                .Add(localItem => localItem.Caption(T("Requests Report")).Action(("PSSRequests"), "PoliceRequest", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewRequests))
                .Add(localItem => localItem.Caption(T("Requests Approval")).Action(("PSSRequestList"), "RequestApproval", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanApproveRequest))
                .Add(localItem => localItem.Caption(T("Update Police Character Certificate Details")).Action(("SearchFileNumber"), "PSSCharacterCertificateDetailsUpdate", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanUpdateDetailsForPCC));
        }

        private void PoliceEGSRegularizationMenu(NavigationItemBuilder menu)
        {
            menu.Permission(Permissions.PoliceEGSRegularizationMenu)
                .LinkToFirstChild(false)
                .Add(localItem => localItem.Caption(T("Search for User Profile")).Action(nameof(RouteName.PSSBranchSubUsersUpload.GetRegularizationProfile), nameof(RouteName.PSSBranchSubUsersUpload), new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewRegularizationProfile));
        }

        private void PoliceUserManagementMenu(NavigationItemBuilder menu)
        {
            menu.Permission(Permissions.PoliceUserManagementMainMenu)
                .LinkToFirstChild(false)
                .Add(localItem => localItem.Caption(T("Create User")).Action(("CreateUser"), "PSSAdminUser", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanCreateAdminUser))
                 .Add(localItem => localItem.Caption(T("View Users")).Action("UserReport", "PSSUserReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewUsers))
                 .Add(localItem => localItem.Caption(T("View Admin Users")).Action(("AdminUserReport"), "PSSAdminUserReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewAdminUsers))
                 .Add(localItem => localItem.Caption(T("Assign Escort Process Flow")).Action(("AssignProcessFlow"), "PSSAdminUserAssignEscortProcessFlow", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanAssignEscortProcessFlow));
        }


        private void PoliceAdminSignatureUploadMenu(NavigationItemBuilder menu)
        {
            menu.Permission(Permissions.PoliceAdminSignatureUploadMainMenu)
                .LinkToFirstChild(false)
                .Add(localItem => localItem.Caption(T("Upload Signature")).Action(("UploadSignature"), "PSSAdminSignatureUpload", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanUploadSignatures))
                .Add(localItem => localItem.Caption(T("Signatures")).Action(("SignaturesList"), "PSSAdminSignatureUpload", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewUploadedSignatures));
        }

        private void PolicePaymentMenu(NavigationItemBuilder menu)
        {
            menu.Permission(Permissions.PolicePaymentMenu)
                .LinkToFirstChild(false)
                .Add(localItem => localItem.Caption(T("Expenditure Head")).Action(("Report"), "PSSExpenditureHeadReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewExpenditureHead))
                .Add(localItem => localItem.Caption(T("Account Wallets")).Action(("Report"), "AccountsWalletReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewAccountWalletReport))
                .Add(localItem => localItem.Caption(T("Payment Approval Module")).Action(("PaymentApproval"), "AccountWalletPaymentApproval", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewWalletPaymentApprovalReport))
                .Add(localItem => localItem.Caption(T("Initiate Wallet Payment Request")).Action(("InitiatePaymentRequest"), "AccountWalletPayment", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanCreateWalletPaymentRequest))
                .Add(localItem => localItem.Caption(T("Initiate Deployment Allowance Payment Request")).Action(("InitiatePaymentRequest"), "DeploymentAllowancePayment", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanCreateDeploymentAllowancePaymentRequest))
                .Add(localItem => localItem.Caption(T("Wallet Payment Report")).Action(("Report"), "AccountWalletPaymentReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewWalletPaymentReport))
                .Add(localItem => localItem.Caption(T("Deployment Allowance Payment Report")).Action(("Report"), "DeploymentAllowancePaymentReport", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewDeploymentAllowancePaymentReport));
        }

        private void AnalyticsDashboardMenu(NavigationItemBuilder menu)
        {
            menu.Add(localItem => localItem.Caption(T("View Analytics Dashboard")).Action(("ViewAnalytics"), "ViewAnalyticsDashboard", new { Area = "Parkway.CBS.Police.Admin" }).Permission(Permissions.CanViewAnalyticsDashboard));
        }

    }
}