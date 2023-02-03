using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;
using System.Collections.Generic;
using Parkway.CBS.Police.Admin.RouteName;

namespace Parkway.CBS.Module
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                //web api route
                new RouteDescriptor {
                    Name = "pss.admin.requests",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Report/Requests", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PoliceRequest"},
                            { "action", "PSSRequests" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "PSS.MDA.Dashboard",
                    Priority = 7,
                    Route = new Route(
                       "Admin", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "Dashboard"},
                            {"action", "MainDashboard"}, //controller action
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PoliceRequest.PSSRequestDetails,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Report/Request/Details/{requestId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PoliceRequest"},
                            { "action", "PSSRequestDetails" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.request.invoices",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Report/Request/Invoices/{requestId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PoliceRequestInvoices"},
                            { "action", "PSSRequestsInvoices" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RequestApproval.PSSRequestList,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/List", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "RequestApproval"},
                            { "action", "PSSRequestList" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RequestApproval.PSSRequestApproval,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Approval/{requestId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "RequestApproval"},
                            { "action", "PSSRequestApproval" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.collection.report",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Reports/Collection/", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSCollectionReport"},
                            { "action", "CollectionReport" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSDeployedOfficersReport.PSSDeployedOfficers,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Reports/Officers-Deployment-Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSDeployedOfficersReport"},
                            { "action", "PSSDeployedOfficers" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = "pss.admin.GetCommandByLGA",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-commands-in-lga", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "Command"},
                            {"action", "CommandsByLGA"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.police.officer.report",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Reports/Officers-Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PoliceOfficerReport"},
                            { "action", "PoliceOfficerReport" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.police.command.report",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Reports/Command-Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "CommandWalletReport"},
                            { "action", "CommandWalletReport" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = CommandWallet.PSSCommandWallet,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Wallet/Command-Wallet", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "CommandWallet"},
                            { "action", "AddCommandWallet" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.police.command.statement.report",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Reports/Command-Statement-Report/{code}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "CommandStatementReport"},
                            { "action", "CommandStatementReport" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.police.deployment.allowance.report",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Reports/Deployment-Allowance-Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSDeploymentAllowanceReport"},
                            { "action", "PSSDeploymentAllowanceReport" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = "pss.admin.GetPoliceOfficersByCommandAndRank",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-police-officers-of-rank-in-command", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PoliceOfficerAJAX"},
                            {"action", "PoliceOfficersByCommandAndRankId"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.change.officer",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Deployment/Change-Officer/{deploymentId}/PoliceOfficer/{officerId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSChangeDeployedOfficer"},
                            { "action", "ChangeDeployedOfficer" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.police.deploymentallowance.request",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Deployment-Allowance/request", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSDeploymentAllowanceRequest"},
                            { "action", "DeploymentAllowanceRequest" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.police.deploymentallowance.requestdetails",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Deployment-Allowance/Approval/{deploymentAllowanceRequestId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSDeploymentAllowanceRequest"},
                            { "action", "RequestDetails" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.police.deploymentallowance.report.requestdetails",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Deployment-Allowance/Report/{deploymentAllowanceRequestId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSDeploymentAllowanceReport"},
                            { "action", "RequestDetails" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RequestApprovalLog.PSSRequestApprovalLog,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Approval-Log/Request/{requestId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "RequestApprovalLog"},
                            { "action", "RequestApprovalLog" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "pss.admin.police.request.preview.approval.log",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Approval-Log-Preview/Request/{requestId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "RequestApprovalLog"},
                            { "action", "PreviewApprovalLog" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = EndOfficerDeployment.PSSEndOfficerDeployment,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Deployment/End-Deployment/{deploymentId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSEndOfficerDeployment"},
                            { "action", "EndOfficerDeployment" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSExtractApproval.ApproveRequest,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Extract/ApproveRequest", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSExtractApproval)},
                            { "action", nameof(PSSExtractApproval.ApproveRequest) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSEscortApproval.ApproveRequest,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Escort/ApproveRequest", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSEscortApproval)},
                            { "action", nameof(PSSEscortApproval.ApproveRequest) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSGenericApproval.ApproveRequest,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Generic/ApproveRequest", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSGenericApproval)},
                            { "action", nameof(PSSGenericApproval.ApproveRequest) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSCharacterCertificateApproval.ApproveRequest,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/CharacterCertificate/ApproveRequest", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSCharacterCertificateApproval)},
                            { "action", nameof(PSSCharacterCertificateApproval.ApproveRequest) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSSettlementReport.ReportSummary,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Report/Summary", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSSettlementReport)},
                            { "action", nameof(PSSSettlementReport.ReportSummary) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSSettlementReport.ReportInvoices,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Report/{batchRef}/Invoices", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSSettlementReport)},
                            { "action", nameof(PSSSettlementReport.ReportInvoices) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSSettlementReport.ReportParties,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Report/{batchRef}/Parties", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSSettlementReport)},
                            { "action", nameof(PSSSettlementReport.ReportParties) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSCharacterCertificateRequestDetails.ViewCertificate,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Details/CharacterCertificate/{fileRefNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSCharacterCertificateRequestDetails"},
                            { "action", "ViewCertificate" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSCharacterCertificateRequestDetails.ViewBiometrics,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Details/CharacterCertificate/Biometrics/{requestId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSCharacterCertificateRequestDetails"},
                            { "action", "ViewFingerPrintBiometrics" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSExtractRequestDetails.ViewExtract,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Details/Extract/{fileRefNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSExtractRequestDetails"},
                            { "action", "ViewExtract" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSRequestApprovalDocumentPreview.ViewDraftServiceDocument,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Approval/View/Draft/Document/{fileRefNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSRequestApprovalDocumentPreview"},
                            { "action", "ViewDraftServiceDocument" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSRequestApprovalDocumentPreviewAJAX.ConfirmAdminHasViewedDraftDocument,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Approval/View/Draft/Document/Confirm/{fileRefNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSRequestApprovalDocumentPreviewAJAX"},
                            { "action", "ConfirmAdminHasViewedDraftDocument" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = "pss.admin.GetPersonnelByServiceNumber",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-personnel-with-service-number", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSEscortApprovalAJAX"},
                            {"action", "GetPersonnelWithServiceNumber"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                     Name = "pss.admin.AdminGetPersonnelByServiceNumber",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-officer-with-service-number", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSAdminUserAJAX"},
                            {"action", "GetPersonnelWithServiceNumber"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = UserManagement.PSSCreateUser,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/UserManagement/CreateUser", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSAdminUser"},
                            { "action", "CreateUser" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = UserManagement.PSSEditUser,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/UserManagement/EditUser/{adminUserId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSAdminUser"},
                            { "action", "EditUser" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = UserManagement.PSSUserReport,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/UserManagement/Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSAdminUserReport"},
                            { "action", "AdminUserReport" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = UserManagement.ToggleUserStatus,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/UserManagement/ToggleUserStatus/{userPartRecordId}/{status}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSAdminUserReport"},
                            { "action", nameof(UserManagement.ToggleUserStatus) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = UserManagement.PSSGetCommandsByCommandCategory,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-commands-by-commandcategory", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "CommandAJAX"},
                            {"action", "CommandsByCommandCategory"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = "pss.admin.GetCommandsForAdminByState",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-commands-for-admin-in-state", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "Command"},
                            {"action", "CommandsByStateForAdmin"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSEscortApproval.ViewFormations,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/View/Allocated/Formations/{squadAllocationId}/{squadAllocationGroupId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSEscortApproval)},
                            { "action", nameof(PSSEscortApproval.ViewFormations) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = "pss.admin.ViewAllocatedOfficers",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/Request/view-allocated-officers", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSEscortApprovalAJAX"},
                            {"action", "GetAllocatedOfficers"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSEscortApprovalAIG.ApproveRequest,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/AIG/Approval", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSEscortApprovalAIG)},
                            { "action", nameof(PSSEscortApprovalAIG.ApproveRequest) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = "pss.admin.NumberOfOfficersRequested",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/Request/requested-number-of-officers", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSEscortApprovalAJAX"},
                            {"action", "GetNumberOfOfficersRequested"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = "pss.admin.GetAreaAndDivisionalCommandsForAdminByLGA",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-area-divisional-commands-for-admin-in-LGA", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "Command"},
                            {"action", "CommandsByLGAForDCPAdmin"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSEscortApprovalDCP.ApproveRequest,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/DCP/Approval", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSEscortApprovalDCP)},
                            { "action", nameof(PSSEscortApprovalDCP.ApproveRequest) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = CommandAJAX.CommandsByParentCodeFormationLevel,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-commands-by-parent-code-formation-level", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "CommandAJAX"},
                            {"action", "CommandsByParentCodeFormationLevel"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSAdminUserAJAX.GetFlowDefinitionsForService,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-flow-definitions-for-service", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSAdminUserAJAX"},
                            {"action", "GetFlowDefinitionsForService"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSAdminUserAJAX.GetApprovalFlowDefinitionLevelsForDefinition,
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/Police/get-approval-flow-definition-levels-for-definition", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSAdminUserAJAX"},
                            {"action", "GetApprovalFlowDefinitionLevelsForDefinition"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSAdminSignatureUpload.UploadSignature,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Signatures/Upload", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSAdminSignatureUpload"},
                            { "action", "UploadSignature" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSAdminSignatureUpload.SignaturesList,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Signatures/List", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSAdminSignatureUpload"},
                            { "action", "SignaturesList" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSSettlementReport.ReportAggregate,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Report/Aggregate", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSSettlementReport)},
                            { "action", nameof(PSSSettlementReport.ReportAggregate) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSSettlementReport.ReportParty,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Report/{batchRef}/Party", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSSettlementReport)},
                            { "action", nameof(PSSSettlementReport.ReportParty) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSSettlementReport.ReportPartyBreakdown,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Report/Party/{batchRef}/Breakdown/{feePartyBatchAggregateId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSSettlementReport)},
                            { "action", nameof(PSSSettlementReport.ReportPartyBreakdown) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSSettlementReport.ReportBatchBreakdown,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Report/{batchRef}/Breakdown", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSSettlementReport)},
                            { "action", nameof(PSSSettlementReport.ReportBatchBreakdown) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSSettlementReport.ReportBreakdown,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Report/Breakdown", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSSettlementReport)},
                            { "action", nameof(PSSSettlementReport.ReportBreakdown) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSEscortRequestDetails.ViewDispatchNote,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Request/Details/Escort/{fileRefNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", "PSSEscortRequestDetails"},
                            { "action", "ViewDispatchNote" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = POSSAPSecretariatRouting.RouteForEscort,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Secretariat/Route/Escort", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(POSSAPSecretariatRouting)},
                            { "action", nameof(POSSAPSecretariatRouting.RouteForEscort) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = POSSAPSecretariatRouting.RouteForCharacterCertificate,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Secretariat/Route/CharacterCertificate", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(POSSAPSecretariatRouting)},
                            { "action", nameof(POSSAPSecretariatRouting.RouteForCharacterCertificate) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = SettlementFeeParties.FeeParties,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Fee-Parties", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(SettlementFeeParties)},
                            { "action", nameof(SettlementFeeParties.FeeParties) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = SettlementFeeParties.AddFeeParty,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Add-Fee-Party", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(SettlementFeeParties)},
                            { "action", nameof(SettlementFeeParties.AddFeeParty) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = SettlementReport.Report,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(SettlementReport)},
                            { "action", nameof(SettlementReport.Report) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = SettlementReportFeeParties.ViewParties,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Parties/{settlementId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(SettlementReportFeeParties)},
                            { "action", nameof(SettlementReportFeeParties.ViewParties) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = SettlementReportFeeParties.EditParties,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Settlement/Parties/Edit/{settlementId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(SettlementReportFeeParties)},
                            { "action", nameof(SettlementReportFeeParties.EditParties) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = PSSExpenditureHead.AddExpenditureHead,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/ExpenditureHead/Add", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSExpenditureHead)},
                            { "action", nameof(PSSExpenditureHead.AddExpenditureHead) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = PSSExpenditureHeadReport.Report,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/ExpenditureHead/Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSExpenditureHeadReport)},
                            { "action", nameof(PSSExpenditureHeadReport.Report) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = PSSExpenditureHead.EditExpenditureHead,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/ExpenditureHead/Edit/{id}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSExpenditureHead)},
                            { "action", nameof(PSSExpenditureHead.EditExpenditureHead) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = PSSExpenditureHead.DisableExpenditureHead,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/ExpenditureHead/Disable/{id}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSExpenditureHead)},
                            { "action", nameof(PSSExpenditureHead.DisableExpenditureHead) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = BranchOfficer.GenerateEscortRequest,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Branch/Officer/GenerateRequest/{id}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(BranchOfficer)},
                            { "action", nameof(BranchOfficer.GenerateEscortRequest) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = BranchOfficer.OfficerValidationResult,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Branch/Officer/Result", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(BranchOfficer)},
                            { "action", nameof(BranchOfficer.OfficerValidationResult) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = BranchOfficer.BranchProfileDetail,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Branch/Profile/Detail/{id}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(BranchOfficer)},
                            { "action", nameof(BranchOfficer.BranchProfileDetail) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = BranchOfficer.UploadBranchOfficer,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Branch/Officer/Upload/{profileId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(BranchOfficer)},
                            { "action", nameof(BranchOfficer.UploadBranchOfficer) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                        Name = BranchOfficerAjax.CheckIfBranchOfficerUploadCompleted,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchOfficer/Upload/Check-Officer-Upload-Validation-Completion-Status", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(BranchOfficerAjax)},
                                { "action", nameof(BranchOfficerAjax.CheckIfBranchOfficerUploadCompleted) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                   new RouteDescriptor {
                    Name = BranchOfficerFileExport.OfficerRequestDownload,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Branch/Officer/Request/Download", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(BranchOfficerFileExport)},
                            { "action", nameof(BranchOfficerFileExport.OfficerRequestDownload) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                }, 
                new RouteDescriptor {
                    Name = BranchOfficer.EscortRequestDetail,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Branch/Officer/Request/Detail/{id}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(BranchOfficer)},
                            { "action", nameof(BranchOfficer.EscortRequestDetail) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = PSSExpenditureHead.EnableExpenditureHead,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/ExpenditureHead/Enable/{id}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(PSSExpenditureHead)},
                            { "action", nameof(PSSExpenditureHead.EnableExpenditureHead) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   new RouteDescriptor {
                    Name = AccountsWalletReport.Report,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Expenditure/Wallet/Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(AccountsWalletReport)},
                            { "action", nameof(AccountsWalletReport.Report) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Name = AccountWalletConfiguration.AddWalletConfiguration,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Expenditure/Wallet/Configuration/{walletAcctId}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(AccountWalletConfiguration)},
                            { "action", nameof(AccountWalletConfiguration.AddWalletConfiguration) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                       new RouteDescriptor {
                    Name = AccountWalletConfigurationAJAX.GetAdminUserDetail,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Expenditure/Wallet/get-admin-user-detail", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(AccountWalletConfigurationAJAX)},
                            { "action", nameof(AccountWalletConfigurationAJAX.GetAdminUserDetail) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Name = AccountWalletPayment.InitiatePaymentRequest,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Expenditure/Wallet/Payment/Request", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(AccountWalletPayment)},
                            { "action", nameof(AccountWalletPayment.InitiatePaymentRequest) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                      new RouteDescriptor {
                    Name = AccountWalletPaymentAJAX.ValidateAccountNumber,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Expenditure/Wallet/validate-account-number", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(AccountWalletPaymentAJAX)},
                            { "action", nameof(AccountWalletPaymentAJAX.ValidateAccountNumber) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                          new RouteDescriptor {
                    Name = AccountWalletPaymentAJAX.GetAccountBalance,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Expenditure/Wallet/account-balance", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(AccountWalletPaymentAJAX)},
                            { "action", nameof(AccountWalletPaymentAJAX.GetAccountBalance) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                      new RouteDescriptor {
                        Name = AccountWalletPaymentApproval.ViewDetail,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Expenditure/Wallet/Payment/Requests/Detail/{paymentId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(AccountWalletPaymentApproval)},
                                { "action", nameof(AccountWalletPaymentApproval.ViewDetail) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                        new MvcRouteHandler())
                },
                        new RouteDescriptor {
                        Name = AccountWalletPaymentApproval.PaymentApproval,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Expenditure/Wallet/Payment/Requests", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(AccountWalletPaymentApproval)},
                                { "action", nameof(AccountWalletPaymentApproval.PaymentApproval) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },

                          new RouteDescriptor {
                        Name = AccountWalletPaymentApproval.Approve,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Expenditure/Wallet/Payment/Request/Approve/{paymentId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(AccountWalletPaymentApproval)},
                                { "action", nameof(AccountWalletPaymentApproval.Approve) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                            new RouteDescriptor {
                        Name = AccountWalletPaymentApproval.Decline,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Expenditure/Wallet/Payment/Request/Decline/{paymentId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(AccountWalletPaymentApproval)},
                                { "action", nameof(AccountWalletPaymentApproval.Decline) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                             new RouteDescriptor {
                        Name = AccountWalletPaymentReport.Report,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Expenditure/Wallet/Payment/Request/Report", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(AccountWalletPaymentReport)},
                                { "action", nameof(AccountWalletPaymentReport.Report) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                             new RouteDescriptor {
                        Name = "pss.admin.get.wallet.statements",
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Mock/Get-Wallet-Statements", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", "MockWalletStatementApi"},
                                { "action", "GetWalletStatements" },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                        new RouteDescriptor {
                        Name = PSSAdminUserAssignEscortProcessFlow.AssignProcessFlow,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Escort/Process/Flow/Assign", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSAdminUserAssignEscortProcessFlow)},
                                { "action", nameof(PSSAdminUserAssignEscortProcessFlow.AssignProcessFlow) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                        new RouteDescriptor {
                        Name = PSSAdminUserAssignEscortProcessFlowAJAX.GetAdminUser,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Escort/Process/Flow/get-admin-user-details", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSAdminUserAssignEscortProcessFlowAJAX)},
                                { "action", nameof(PSSAdminUserAssignEscortProcessFlowAJAX.GetAdminUser) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                        new RouteDescriptor {
                        Name = PSSAdminUserAssignEscortProcessFlowAJAX.GetEscortProcessStageDefinitions,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Escort/Process/Flow/get-escort-process-stages", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSAdminUserAssignEscortProcessFlowAJAX)},
                                { "action", nameof(PSSAdminUserAssignEscortProcessFlowAJAX.GetEscortProcessStageDefinitions) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                        new RouteDescriptor {
                        Name = ChangePassportPhoto.ChangePhoto,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/PCC/Change-Passport-Photo", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(ChangePassportPhoto)},
                                { "action", nameof(ChangePassportPhoto.ChangePhoto) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                         new RouteDescriptor {
                        Name = ChangePassportPhotoAJAX.GetFileNumberDetailsForPCC,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/PCC/X/Change-Passport-Photo/Get-File-NumberChange-Passport-Photo", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(ChangePassportPhotoAJAX)},
                                { "action", nameof(ChangePassportPhotoAJAX.GetFileNumberDetailsForPCC) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                         new RouteDescriptor {
                        Name = PSSCharacterCertificateDetailsUpdate.SearchFileNumber,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/PCC/Search-FileNumber", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSCharacterCertificateDetailsUpdate)},
                                { "action", nameof(PSSCharacterCertificateDetailsUpdate.SearchFileNumber) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                         new RouteDescriptor {
                        Name = PSSCharacterCertificateDetailsUpdate.UpdateDetails,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/PCC/Update-Details/{fileNumber}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSCharacterCertificateDetailsUpdate)},
                                { "action", nameof(PSSCharacterCertificateDetailsUpdate.UpdateDetails) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                         new RouteDescriptor {
                        Name = PSSSettlementReportFileExport.ReportBreakdownDownload,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/Settlement/Report/Breakdown/Export", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSSettlementReportFileExport)},
                                { "action", nameof(PSSSettlementReportFileExport.ReportBreakdownDownload) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = PSSUserReport.UserReport,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/User/Report", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSUserReport)},
                                { "action", nameof(PSSUserReport.UserReport) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = PSSUserReport.RevalidateUser,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/User/Report/Revalidate-User/{payerId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSUserReport)},
                                { "action", nameof(PSSUserReport.RevalidateUser) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = PSSBranchSubUsersUpload.GetRegularizationProfile,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchSubUsers/Upload/Get-Regularization-Profile", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSBranchSubUsersUpload)},
                                { "action", nameof(PSSBranchSubUsersUpload.GetRegularizationProfile) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = PSSBranchSubUsersUpload.ViewBranchDetails,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchSubUsers/Upload/View-Branch-Details/{payerId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSBranchSubUsersUpload)},
                                { "action", nameof(PSSBranchSubUsersUpload.ViewBranchDetails) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = PSSBranchSubUsersUpload.FileUploadValidation,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchSubUsers/Upload/File-Upload/{payerId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSBranchSubUsersUpload)},
                                { "action", nameof(PSSBranchSubUsersUpload.FileUploadValidation) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = PSSBranchSubUsersUpload.FileUploadValidationResult,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchSubUsers/Upload/View-File-Validation-Result", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSBranchSubUsersUpload)},
                                { "action", nameof(PSSBranchSubUsersUpload.FileUploadValidationResult) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = PSSBranchSubUsersUploadAJAX.CheckIfBranchSubUsersUploadCompleted,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchSubUsers/Upload/Check-Upload-Validation-Completion-Status", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSBranchSubUsersUploadAJAX)},
                                { "action", nameof(PSSBranchSubUsersUploadAJAX.CheckIfBranchSubUsersUploadCompleted) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = PSSBranchSubUsersUpload.SaveBranchSubUsers,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchSubUsers/Upload/Save-Branch-Sub-Users", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(PSSBranchSubUsersUpload)},
                                { "action", nameof(PSSBranchSubUsersUpload.SaveBranchSubUsers) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = BranchOfficerAjax.GetServiceCategoryTypes,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchOfficer/Get-Service-Category-Types", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(BranchOfficerAjax)},
                                { "action", nameof(BranchOfficerAjax.GetServiceCategoryTypes) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = BranchOfficerAjax.GetTacticalSquads,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchOfficer/Get-Escort-Tactical-Squads", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(BranchOfficerAjax)},
                                { "action", nameof(BranchOfficerAjax.GetTacticalSquads) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = BranchOfficerAjax.GetNextLevelCommandsWithCode,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchOfficer/Get-Next-Level-Commands-With-Code", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(BranchOfficerAjax)},
                                { "action", nameof(BranchOfficerAjax.GetNextLevelCommandsWithCode) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = BranchOfficerAjax.GetStateFormations,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/BranchOfficer/Get-State-Formations", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(BranchOfficerAjax)},
                                { "action", nameof(BranchOfficerAjax.GetStateFormations) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateRequestWithoutOfficers.GenerateRequestForDefaultBranchWithoutOfficersUpload,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateRequestWithoutOfficers/Generate-Request-Without-Officers-Upload/{profileId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateRequestWithoutOfficers)},
                                { "action", nameof(GenerateRequestWithoutOfficers.GenerateRequestForDefaultBranchWithoutOfficersUpload) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersUpload,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateRequestWithoutOfficers/Generate-Request-Without-Officers-Upload/Branch/{branchId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateRequestWithoutOfficers)},
                                { "action", nameof(GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersUpload) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateRequestWithoutOfficers.GenerateRequestForDefaultBranchWithoutOfficersFileUploadValidation,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateRequestWithoutOfficers/Generate-Request-Without-Officers-Upload-Validation/{profileId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateRequestWithoutOfficers)},
                                { "action", nameof(GenerateRequestWithoutOfficers.GenerateRequestForDefaultBranchWithoutOfficersFileUploadValidation) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateRequestWithoutOfficers.GenerateRequestForDefaultBranchWithoutOfficersFileUploadValidationResult,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateRequestWithoutOfficers/Generate-Request-Without-Officers-Upload-Validation-Result", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateRequestWithoutOfficers)},
                                { "action", nameof(GenerateRequestWithoutOfficers.GenerateRequestForDefaultBranchWithoutOfficersFileUploadValidationResult) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateRequestWithoutOfficersAJAX.CheckIfBatchUploadCompleted,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateRequestWithoutOfficers/Upload/Check-Upload-Validation-Completion-Status", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateRequestWithoutOfficersAJAX)},
                                { "action", nameof(GenerateRequestWithoutOfficersAJAX.CheckIfBatchUploadCompleted) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersFileUploadValidation,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateRequestWithoutOfficers/Generate-Request-Without-Officers-Upload-Validation/Branch/{branchId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateRequestWithoutOfficers)},
                                { "action", nameof(GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersFileUploadValidation) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersFileUploadValidationResult,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateRequestWithoutOfficers/Generate-Request-Without-Officers-Upload-Validation-Result/Branch", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateRequestWithoutOfficers)},
                                { "action", nameof(GenerateRequestWithoutOfficers.GenerateRequestForBranchWithoutOfficersFileUploadValidationResult) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateRequestWithoutOfficersFileExport.GenerateBranchRequestWithoutOfficersDownload,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateRequestWithoutOfficers/Branch/Download", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateRequestWithoutOfficersFileExport)},
                                { "action", nameof(GenerateRequestWithoutOfficersFileExport.GenerateBranchRequestWithoutOfficersDownload) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateRequestWithoutOfficersFileExport.GenerateDefaultRequestWithoutOfficersDownload,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateRequestWithoutOfficers/Default/Download", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateRequestWithoutOfficersFileExport)},
                                { "action", nameof(GenerateRequestWithoutOfficersFileExport.GenerateDefaultRequestWithoutOfficersDownload) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateEscortRequestWithoutOfficers.GenerateEscortRequestForDefaultBranchWithoutOfficers,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateEscortRequestWithoutOfficers/Generate-Request-Without-Officers/{batchId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateEscortRequestWithoutOfficers)},
                                { "action", nameof(GenerateEscortRequestWithoutOfficers.GenerateEscortRequestForDefaultBranchWithoutOfficers) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = GenerateEscortRequestWithoutOfficers.GenerateEscortRequestForBranchWithoutOfficers,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/GenerateEscortRequestWithoutOfficers/Generate-Request-Without-Officers/Branch/{batchId}", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(GenerateEscortRequestWithoutOfficers)},
                                { "action", nameof(GenerateEscortRequestWithoutOfficers.GenerateEscortRequestForBranchWithoutOfficers) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                    new RouteDescriptor {
                    Name = DeploymentAllowancePayment.InitiatePaymentRequest,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Expenditure/Deployment/Allowance/Payment/Request", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(DeploymentAllowancePayment)},
                            { "action", nameof(DeploymentAllowancePayment.InitiatePaymentRequest) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Name = DeploymentAllowancePaymentAJAX.GetPersonnelAccountName,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Expenditure/Deployment/Allowance/Payment/get-personnel-account-name", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(DeploymentAllowancePaymentAJAX)},
                            { "action", nameof(DeploymentAllowancePaymentAJAX.GetPersonnelAccountName) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Name = DeploymentAllowancePaymentAJAX.ComputeAmountForPersonnel,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Expenditure/Deployment/Allowance/Payment/compute-personnel-amount", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(DeploymentAllowancePaymentAJAX)},
                            { "action", nameof(DeploymentAllowancePaymentAJAX.ComputeAmountForPersonnel) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Name = DeploymentAllowancePaymentReport.Report,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Deployment/Allowance/Payment/Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(DeploymentAllowancePaymentReport)},
                            { "action", nameof(DeploymentAllowancePaymentReport.Report) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Name = DeploymentAllowancePaymentReportExport.Download,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Deployment/Allowance/Payment/Report/Download", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(DeploymentAllowancePaymentReportExport)},
                            { "action", nameof(DeploymentAllowancePaymentReportExport.Download) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                    new RouteDescriptor {
                    Name = ChangePCCBioData.ChangeBioData,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/PCC/Change-Passport-Bio-Data-page", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                            {"controller", nameof(ChangePCCBioData)},
                            { "action", nameof(ChangePCCBioData.ChangeBioData) },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                           new RouteDescriptor {
                        Name = AccountWalletPaymentApprovalAJAX.GetToken,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/AccountWalletPaymentApprovalAJAX/Get-Token", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(AccountWalletPaymentApprovalAJAX)},
                                { "action", nameof(AccountWalletPaymentApprovalAJAX.GetToken) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = AccountWalletPaymentApprovalAJAX.ValidateVerificationCode,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/AccountWalletPaymentApprovalAJAX/Validate-Verification-Code", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(AccountWalletPaymentApprovalAJAX)},
                                { "action", nameof(AccountWalletPaymentApprovalAJAX.ValidateVerificationCode) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
                           new RouteDescriptor {
                        Name = AccountWalletPaymentApprovalAJAX.ResendVerificationCode,
                        Priority = 5,
                        Route = new Route(
                           "Admin/Police/AccountWalletPaymentApprovalAJAX/Resend-Verification-Code", // this is the name of the page url
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"}, // this is the name of your module
                                {"controller", nameof(AccountWalletPaymentApprovalAJAX)},
                                { "action", nameof(AccountWalletPaymentApprovalAJAX.ResendVerificationCode) },
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "Parkway.CBS.Police.Admin"} // this is the name of your module
                            },
                            new MvcRouteHandler())
                         },
            };
        }


    }
}