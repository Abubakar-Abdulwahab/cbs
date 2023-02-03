using Orchard.Mvc.Routes;
using Parkway.CBS.Module.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.RSTVL.Web
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var routes = BaseRoutes.GetRoutes("R.", "RSTVL");
            return routes;
        }
    }
}