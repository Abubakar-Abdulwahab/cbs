using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module
{
    public class Permissions : IPermissionProvider
    {
        //if you can create you can view
        //if u have the create permission you can view, if u only have the view permission u cannot create
        public static readonly Permission CreateMDA = new Permission { Description = "Create a new MDA", Name = "CreateMDA" };

        public static readonly Permission CreateRevenueHead = new Permission { Description = "Create revenue heads", Name = "CreateRevenueHead" };
        public static readonly Permission CreateBilling = new Permission { Description = "Create billing details", Name = "CreateBilling" };
        public static readonly Permission CreateForm = new Permission { Description = "Create form details", Name = "CreateForm" };
        public static readonly Permission ViewRevHeadDashBoard = new Permission { Description = "Create form details", Name = "ViewRevHeadDashBoard" };

        public static readonly Permission CreateAdminUser = new Permission { Description = "Create an admin user", Name = "CreateAdminUser", };
        public static readonly Permission ManageAdminSettings = new Permission { Description = "Manage Admin settings", Name = "ManageAdminSettings" };
        public static readonly Permission AddPayments = new Permission { Description = "Add Payments to an invoice Admin", Name = "AddPayments" };

        public static readonly Permission ViewReceipt = new Permission { Description = "View Receipt per transactions", Name = "ViewReceipt" };

        public static readonly Permission GenerateInvoice = new Permission { Description = "Generate invoice from back office", Name = "GenerateInvoice" };

        public static readonly Permission ViewInvoiceReport = new Permission { Description = "View invoice report", Name = "ViewInvoiceReport" };

        public static readonly Permission ViewTaxPayersReport = new Permission { Description = "View tax payers report", Name = "ViewTaxPayersReport" };

        public static readonly Permission ViewCollectionReport = new Permission { Description = "View collection report", Name = "ViewCollectionReport" };

        public static readonly Permission ViewRevenueHeadList = new Permission { Description = "View revenue head list", Name = "ViewRevenueHeadList" };

        public static readonly Permission ViewMDAList = new Permission { Description = "View MDAs", Name = "ViewMDAList", ImpliedBy = new[] { CreateMDA } };

        public static readonly Permission ViewAccountStatement = new Permission { Description = "View tax payer account statement", Name = "ViewAccountStatement" };

        public static readonly Permission UpdateBillingAmount = new Permission { Description = "Update billing amount", Name = "UpdateBillingAmount" };

        public static readonly Permission EditTaxPayer = new Permission { Description = "Edit tax payer details", Name = "EditTaxPayer" };

        public static readonly Permission CreateSettlementRule = new Permission { Description = "Create settlement rule", Name = "CreateSettlementRule" };

        //public static readonly Permission Create = new Permission { Description = "Create a new MDA", Name = "Create" };
        //public static readonly Permission AccessPersonListDashBoard = new Permission { Description = "Access the Person List Dashboard", Name = "AccessPersonListDashBoard", ImpliedBy = new[] { EditPersonList } };

        public static readonly Permission CreateExternalPaymentProvider = new Permission { Description = "Create external payment provider details", Name = "CreateExternalPaymentProvider" };

        public static readonly Permission ViewExternalPaymentProvider = new Permission { Description = "View external payment provider details", Name = "ViewExternalPaymentProvider" };

        public static readonly Permission AssignExternalPaymentProvider = new Permission { Description = "Assign constraints to external payment provider", Name = "AssignExternalPaymentProvider" };

        public static readonly Permission AssignRevenueHeadPermissions = new Permission { Description = "Assign revenue head constraints to expert systems", Name = "AssignRevenueHeadPermissions" };

        public static readonly Permission RevenueHeadMainMenu = new Permission { Description = "Give access to the revenue head main menu", Name = "RevenueHeadMainMenu" };

        public static readonly Permission TenantSettingMainMenu = new Permission { Description = "Give access to the tenant settings main menu", Name = "TenantSettingMainMenu" };

        public static readonly Permission ReportMainMenu = new Permission { Description = "Give access to the report main menu", Name = "ReportMainMenu" };

        public static readonly Permission PaymentProviderMainMenu = new Permission { Description = "Give access to the payment provider main menu", Name = "PaymentProviderMainMenu" };

        public static readonly Permission InvoiceSettingMainMenu = new Permission { Description = "Give access to the invoice settings main menu", Name = "InvoiceSettingMainMenu" };

        public static readonly Permission ViewTINReport = new Permission { Description = "Create View TIN Applicant Report", Name = "ViewTINReport" };

        public static readonly Permission SearchPaymentReference = new Permission { Description = "Search for invoice payment reference", Name = "SearchPaymentReference" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype() {Name = "Administrator", Permissions = new[]
                { CreateMDA, CreateRevenueHead, CreateBilling, CreateForm, ViewRevHeadDashBoard, CreateAdminUser, ManageAdminSettings, ViewRevenueHeadList , ViewMDAList, UpdateBillingAmount, ViewAccountStatement, EditTaxPayer} },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { CreateRevenueHead } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { CreateBilling } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { CreateForm } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { ViewRevHeadDashBoard } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { CreateAdminUser } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { ManageAdminSettings } },
                new PermissionStereotype() {Name = "Administrator", Permissions = new[]
                { AddPayments, GenerateInvoice, ViewReceipt, ViewInvoiceReport, ViewTaxPayersReport, ViewCollectionReport, CreateSettlementRule } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { GenerateInvoice } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { ViewTaxPayerReport } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { ViewReceipt } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { ViewInvoiceReport } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { ViewTaxPayersReport } },
                //new PermissionStereotype() {Name = "Administrator", Permissions = new[] { ViewCollectionReport } },
            };
        }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                CreateMDA,
                CreateRevenueHead,
                CreateBilling,
                CreateForm,
                ViewRevHeadDashBoard,
                CreateAdminUser,
                ManageAdminSettings,
                AddPayments,
                GenerateInvoice,
                ViewReceipt,
                ViewInvoiceReport,
                ViewTaxPayersReport,
                ViewCollectionReport,
                ViewRevenueHeadList,
                ViewMDAList,
                UpdateBillingAmount,
                ViewAccountStatement,
                EditTaxPayer,
                CreateExternalPaymentProvider,
                ViewExternalPaymentProvider,
                AssignExternalPaymentProvider,
                CreateSettlementRule,
                AssignRevenueHeadPermissions,
                RevenueHeadMainMenu,
                TenantSettingMainMenu,
                ReportMainMenu,
                PaymentProviderMainMenu,
                InvoiceSettingMainMenu,
                ViewTINReport,
                SearchPaymentReference
            };
        }
    }
}