using System.Collections.Generic;
using Orchard.Security.Permissions;
using Orchard.Environment.Extensions.Models;

namespace Parkway.CBS.Police.Admin
{
    public class Permissions : IPermissionProvider
    {
        //if you can create you can view
        //if u have the create permission you can view, if u only have the view permission u cannot create
        public static readonly Permission CanViewRanks = new Permission { Description = "Can view Ranks", Name = "CanViewRanks" };
        public static readonly Permission CanViewRequests = new Permission { Description = "Can view request", Name = "CanViewRequests" };
        public static readonly Permission CanViewCollectionReports = new Permission { Description = "Can view collection reports", Name = "CanViewCollectionReports" };
        public static readonly Permission CanViewDeployments = new Permission { Description = "Can view officers deployments reports", Name = "CanViewDeployments" };
        public static readonly Permission CanViewOfficers = new Permission { Description = "Can view officers reports", Name = "CanViewOfficers" };
        public static readonly Permission CanApproveRequest = new Permission { Description = "Can approve requests", Name = "CanApproveRequest" };
        public static readonly Permission CanViewDeploymentAllowanceRequests = new Permission { Description = "Can view officer deployment allowance requests", Name = "CanViewDeploymentAllowanceRequests" };
        public static readonly Permission CanViewDeploymentAllowanceReports = new Permission { Description = "Can view officer deployment allowance reports", Name = "CanViewDeploymentAllowanceReports" };
        public static readonly Permission CanViewCommandWalletReports = new Permission { Description = "Can view command reports", Name = "CanViewCommandReports" };
        public static readonly Permission CanViewSettlementReportSummary = new Permission { Description = "Can view settlement report summary", Name = "CanViewSettlementReportSummary" };
        public static readonly Permission CanViewSettlements = new Permission { Description = "Can view settlements", Name = "CanViewSettlements" };
        public static readonly Permission CanViewSettlementReportParties = new Permission { Description = "Can view settlement report parties", Name = "CanViewSettlementReportParties" };
        public static readonly Permission CanEditSettlementReportParties = new Permission { Description = "Can edit settlement report parties", Name = "CanEditSettlementReportParties" };
        public static readonly Permission CanViewSettlementReportPartyBreakdown = new Permission { Description = "Can view settlement report party breakdown", Name = "CanViewSettlementReportPartyBreakdown" };
        public static readonly Permission CanViewSettlementReportBatchBreakdown = new Permission { Description = "Can view settlement report batch breakdown", Name = "CanViewSettlementReportBatchBreakdown" };
        public static readonly Permission CanApproveDeploymentAllowance = new Permission { Description = "Can approve deployment allowance requests", Name = "CanApproveDeploymentAllowance" };
        public static readonly Permission CanEndOfficerDeployment = new Permission { Description = "Can end officer deployment", Name = "CanEndOfficerDeployment" };
        public static readonly Permission PoliceRequestMainMenu = new Permission { Description = "Give access to the police main menu", Name = "PoliceRequestMainMenu" };
        public static readonly Permission PoliceUserManagementMainMenu = new Permission { Description = "Give access to the police user management main menu", Name = "PoliceUserManagementMainMenu" };
        public static readonly Permission PoliceEGSRegularizationMenu = new Permission { Description = "Give access to the police escort and guards regularization main menu", Name = "PoliceEGSRegularizationMenu" };
        public static readonly Permission CanCreateAdminUser = new Permission { Description = "Can create admin user", Name = "CanCreateAdminUser" };
        public static readonly Permission CanViewAdminUsers = new Permission { Description = "Can view admin user", Name = "CanViewAdminUsers" };
        public static readonly Permission CanViewUsers = new Permission { Description = "Can view user", Name = "CanViewUsers" };
        public static readonly Permission PoliceAdminSignatureUploadMainMenu = new Permission { Description = "Give access to the police admin signature upload main menu", Name = "PoliceAdminSignatureUploadMainMenu" };
        public static readonly Permission CanUploadSignatures = new Permission { Description = "Can upload signatures", Name = "CanUploadSignatures" };
        public static readonly Permission CanViewUploadedSignatures = new Permission { Description = "Can view uploaded signatures", Name = "CanViewUploadedSignatures" };
        public static readonly Permission CanEditAdminUsers = new Permission { Description = "Can edit admin user", Name = "CanEditAdminUsers" };
        public static readonly Permission CanCreateCommandWallet = new Permission { Description = "Can create command wallet", Name = "CanCreateCommandWallet" };
        public static readonly Permission CanViewSettlementFeeParties = new Permission { Description = "Can view settlement fee parties", Name = "CanViewSettlementFeeParties" };
        public static readonly Permission CanAddSettlementFeeParties = new Permission { Description = "Can add settlement fee parties", Name = "CanAddSettlementFeeParties" };
        public static readonly Permission CanAddExpenditureHead = new Permission { Description = "Can add expenditure head", Name = "CanAddExpenditureHead" };
        public static readonly Permission CanViewExpenditureHead = new Permission { Description = "Can view expenditure head", Name = "CanViewExpenditureHead" };
        public static readonly Permission PolicePaymentMenu = new Permission { Description = "Can view police payment menu", Name = "PolicePaymentMenu" };
        public static readonly Permission CanViewAccountWalletReport = new Permission { Description = "Can view account wallet report", Name = "CanViewAccountWalletReport" };
        public static readonly Permission CanAddWalletConfiguration = new Permission { Description = "Can add wallet configuration", Name = "CanAddWalletConfiguration" };
        public static readonly Permission CanCreateWalletPaymentRequest = new Permission { Description = "Can initiate wallet payment request", Name = "CanCreateWalletPaymentRequest" };
        public static readonly Permission CanCreateDeploymentAllowancePaymentRequest = new Permission { Description = "Can initiate deployment allowance payment request", Name = "CanCreateDeploymentAllowancePaymentRequest" };
        public static readonly Permission CanViewDeploymentAllowancePaymentReport = new Permission { Description = "Can view deployment allowance payment report", Name = "CanViewDeploymentAllowancePaymentReport" };
        public static readonly Permission CanViewWalletPaymentApprovalReport = new Permission { Description = "Can View Wallet Payment Approval Report", Name = "CanViewWalletPaymentApprovalReport" };
        public static readonly Permission CanViewWalletPaymentReport = new Permission { Description = "Can View Wallet Payment Report", Name = "CanViewWalletPaymentReport" };
        public static readonly Permission CanAssignEscortProcessFlow = new Permission { Description = "Can Assign Escort Process Flow", Name = "CanAssignEscortProcessFlow" };
        public static readonly Permission CanChangePassportPhotoForPCC = new Permission { Description = "Can change passport photo for PCC", Name = "CanChangePassportPhotoForPCC" };
        public static readonly Permission CanUpdateDetailsForPCC = new Permission { Description = "Can update details for PCC", Name = "CanUpdateDetailsForPCC" };
        public static readonly Permission CanViewAnalyticsDashboard = new Permission { Description = "Can view analytics dashboard", Name = "CanViewAnalyticsDashboard" };
        public static readonly Permission CanViewRegularizationProfile = new Permission { Description = "Can view regularization profile", Name = "CanViewRegularizationProfile" };
        public static readonly Permission CanViewUploadBranchSubUsers = new Permission { Description = "Can view upload branch sub users", Name = "CanViewUploadBranchSubUsers" };
        public static readonly Permission CanSaveUploadedBranchSubUsers = new Permission { Description = "Can save uploaded branch sub users", Name = "CanSaveUploadedBranchSubUsers" };
        public static readonly Permission CanViewBranchProfileDetail = new Permission { Description = "Can view branch profile", Name = "CanViewBranchProfileDetail" };
        public static readonly Permission CanDownloadBranchProfileOfficerRequest = new Permission { Description = "Can download branch profile officer request", Name = "CanDownloadBranchProfileOfficerRequest" };
        public static readonly Permission CanViewEscortRequestDetail = new Permission { Description = "Can view escort request detail", Name = "CanViewEscortRequestDetail" };
        public static readonly Permission CanGenerateBranchEscortRequest = new Permission { Description = "Can generate branch escort request", Name = "CanGenerateBranchEscortRequest" };
        public static readonly Permission CanChangePassportBioDataPageForPCC = new Permission { Description = "Can change passport bio data page for PCC", Name = "CanChangePassportBioDataPageForPCC" };
        public static readonly Permission CanRevalidateUser = new Permission { Description = "Can revalidate user", Name = "CanRevalidateUser" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype() {Name = "Administrator", Permissions = new[]
                { CanViewRanks, CanViewRequests, CanApproveRequest, CanViewDeploymentAllowanceRequests, CanViewSettlementReportSummary, CanEndOfficerDeployment} },
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                CanViewRanks,
                CanViewRequests,
                CanViewCollectionReports,
                CanViewCommandWalletReports,
                CanViewDeployments,
                CanViewOfficers,
                CanApproveRequest,
                CanViewDeploymentAllowanceRequests,
                CanViewDeploymentAllowanceReports,
                CanViewSettlementReportSummary,
                CanViewSettlements,
                CanViewSettlementReportParties,
                CanEditSettlementReportParties,
                CanViewSettlementReportPartyBreakdown,
                CanViewSettlementReportBatchBreakdown,
                CanApproveDeploymentAllowance,
                CanEndOfficerDeployment,
                PoliceRequestMainMenu,
                PoliceUserManagementMainMenu,
                PoliceEGSRegularizationMenu,
                CanCreateAdminUser,
                PoliceAdminSignatureUploadMainMenu,
                CanUploadSignatures,
                CanViewUploadedSignatures,
                CanViewAdminUsers,
                CanEditAdminUsers,
                CanCreateCommandWallet,
                CanViewSettlementFeeParties,
                CanAddSettlementFeeParties,
                CanAddExpenditureHead,
                CanViewExpenditureHead,
                PolicePaymentMenu,
                CanViewAccountWalletReport,
                CanAddWalletConfiguration,
                CanCreateWalletPaymentRequest,
                CanCreateDeploymentAllowancePaymentRequest,
                CanViewWalletPaymentApprovalReport,
                CanViewWalletPaymentReport,
                CanAssignEscortProcessFlow,
                CanChangePassportPhotoForPCC,
                CanUpdateDetailsForPCC,
                CanViewAnalyticsDashboard,
                CanViewUploadBranchSubUsers,
                CanViewRegularizationProfile,
                CanSaveUploadedBranchSubUsers,
                CanViewBranchProfileDetail,
                CanDownloadBranchProfileOfficerRequest,
                CanViewEscortRequestDetail,
                CanGenerateBranchEscortRequest,
                CanViewDeploymentAllowancePaymentReport,
                CanChangePassportBioDataPageForPCC,
                CanRevalidateUser
            };
        }
    }
}