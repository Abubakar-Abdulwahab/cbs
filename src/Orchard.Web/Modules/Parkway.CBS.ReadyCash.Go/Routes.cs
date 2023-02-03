using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.ReadyCash.Go
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
            var readyCashRoutes = new[] {

                   new RouteDescriptor {
                    Name = "C.Form.Fields",
                    Priority = 5,
                    Route = new Route(
                       "c/form-fields",
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.ReadyCash.Go"}, // this is the name of your module
                            {"controller", "RenderFormFields"},
                            {"action", "RenderFormField"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", $"Parkway.CBS.ReadyCash.Go"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                }
            };

            //var routes = BaseRoutes.GetRoutes("", "Client");
            //routes.AddRange(clientRoutes);
            return readyCashRoutes;

        }
    }
}