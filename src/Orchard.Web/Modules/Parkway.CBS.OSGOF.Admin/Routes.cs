using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.OSGOF.Admin
{
    public class Routes : IRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] 
            {
                new RouteDescriptor {
                    Name = "AddOperator",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Operator/AddOperator",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"}, // this is the name of your module
                            {"controller", "Operator"},
                            {"action", "AddOperator"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = "SearchOperator",
                    Priority = 5,
                    Route = new Route(
                       "Admin/Operator/SearchOperator",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"}, // this is the name of your module
                            {"controller", "Operator"},
                            {"action", "SearchOperator"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = "AddCellSites",
                    Priority = 5,
                    Route = new Route(
                       "Admin/CellSites/{payerId}/AddCellSites",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"}, // this is the name of your module
                            {"controller", "CellSites"},
                            {"action", "AddCellSites"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = "CellSitesFileUploadReport",
                    Priority = 5,
                    Route = new Route(
                       "Admin/CellSites/{scheduleRef}/Report",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"}, // this is the name of your module
                            {"controller", "CellSites"},
                            {"action", "CellSitesFileUploadReport"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                 new RouteDescriptor {
                    Name = "List",
                    Priority = 5,
                    Route = new Route(
                       "Admin/CellSites/Report",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"}, // this is the name of your module
                            {"controller", "CellSites"},
                            {"action", "List"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.OSGOF.Admin"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }
    }
}