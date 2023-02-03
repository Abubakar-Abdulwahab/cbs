using Orchard.Mvc.Routes;
using Parkway.CBS.ETCC.Admin.RouteName;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.ETCC.Admin
{
    public class Routes : IRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor {
                    Name = "tcc.admin.request.report",
                    Priority = 5,
                    Route = new Route(
                       "Admin/TCC/Report/Requests",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"}, // this is the name of your module
                            {"controller", "TCCReport"},
                            {"action", "RequestReport"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "tcc.admin.request.report.details",
                    Priority = 5,
                    Route = new Route(
                       "Admin/TCC/Report/Request/Details/{applicationNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"}, // this is the name of your module
                            {"controller", "TCCReport"},
                            { "action", "TCCRequestDetails" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RequestApproval.TCCRequestApprovalList,
                    Priority = 5,
                    Route = new Route(
                       "Admin/TCC/Request/List", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"}, // this is the name of your module
                            {"controller", "RequestApproval"},
                            { "action", "TCCRequests" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RequestApproval.TCCRequestApprovalDetails,
                    Priority = 5,
                    Route = new Route(
                       "Admin/TCC/Request/Details/{applicationNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"}, // this is the name of your module
                            {"controller", "RequestApproval"},
                            { "action", "TCCRequestDetails" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                   //reports
                new RouteDescriptor {
                    Name = "DirectAssessmentReport",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "Admin/PAYE/Report/DirectAssessment", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"}, // this is the name of your module
                            {"controller", "DirectAssessmentReport"},
                            {"action", "DirectAssessmentRequestReport"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "tcc.admin.view.certificate",
                    Priority = 5,
                    Route = new Route(
                       "Admin/TCC/Certificate/{tccNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"}, // this is the name of your module
                            {"controller", "TCCReport"},
                            { "action", "ViewCertificate" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "tcc.admin.paye.receipts.report",
                    Priority = 5,
                    Route = new Route(
                       "Admin/TCC/PAYE/Receipts/Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"}, // this is the name of your module
                            {"controller", "ReceiptUtilizationReport"},
                            { "action", "Receipts" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "tcc.admin.paye.receipts.utilizations",
                    Priority = 5,
                    Route = new Route(
                       "Admin/TCC/PAYE/Receipts/utilizations/{receiptNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"}, // this is the name of your module
                            {"controller", "ReceiptUtilizationReport"},
                            { "action", "Utilizations" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RequestApprovalLog.TCCRequestApprovalLogs,
                    Priority = 5,
                    Route = new Route(
                       "Admin/TCC/Approval-Log-Preview/Request/{applicationNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"}, // this is the name of your module
                            {"controller", "RequestApprovalLog"},
                            { "action", "PreviewApprovalLog" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.ETCC.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }
    }
}