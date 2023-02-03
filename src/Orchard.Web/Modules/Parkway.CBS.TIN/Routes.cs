using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.TIN
{
    public class Routes : IRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                Name = "C.GenerateTin",
                Priority = 4,
                Route = new Route(
                       "c/generatetin",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.TIN"}, // this is the name of your module
                            {"controller", "TIN"},
                            {"action", "TIN"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.TIN"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "TINReport",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "Admin/Reports/TINReport", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.TIN"}, // this is the name of your module
                            {"controller", "TINReport"},
                            {"action", "Index"},
                            },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.TIN"} // this is the name of your module
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