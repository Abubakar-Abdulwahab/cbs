using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.POSSAP.Scheduler
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
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerReport.Report,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer/Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerSchedulerReport"},
                            { "action", "Report" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerDeploymentReport.Report,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer-Deployment/Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerDeploymentReport"},
                            { "action", "Report" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerSchedulingReport.Report,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer-Scheduling/Report", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerSchedulingReport"},
                            { "action", "Report" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerSchedulingRequest.RequestDetails,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer-Scheduling/Request-Details", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerSchedulingRequest"},
                            { "action", "RequestDetails" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerDeploymentReport.ReplaceOfficer,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer-Deployment/Replace-Officer", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerDeploymentReport"},
                            { "action", "ReplaceOfficer" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerDeploymentReport.EndDeployment,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer-Deployment/End-Deployment", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerDeploymentReport"},
                            { "action", "EndDeployment" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerDeploymentReport.DeploymentHistory,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer-Deployment/Deployment-History", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerDeploymentReport"},
                            { "action", "DeploymentHistory" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerSchedulerReportAJAX.GetData,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer/Get-Data", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerSchedulerReportAJAX"},
                            { "action", "GetData" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerSchedulerReportAJAX.CheckConstraints,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer/Check-Constraints", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerSchedulerReportAJAX"},
                            { "action", "CheckConstraints" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerSchedulerReportAJAX.GetReportData,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer/Get-Report-Data", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerSchedulerReportAJAX"},
                            { "action", "GetReportData" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerSchedulerReportAJAX.GetExternalReportData,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer/Get-External-Report-Data", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerSchedulerReportAJAX"},
                            { "action", "GetExternalReportData" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = RouteName.PoliceOfficerSchedulerReportAJAX.GetPager,
                    Priority = 5,
                    Route = new Route(
                       "Admin/Police/Scheduling/Police-Officer/Get-Pager", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"}, // this is the name of your module
                            {"controller", "PoliceOfficerSchedulerReportAJAX"},
                            { "action", "GetPager" },
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.POSSAP.Scheduler"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
            };
        }
    }
}