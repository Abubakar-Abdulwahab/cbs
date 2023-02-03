using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Web.Payment
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
                //C.HomePage
                new RouteDescriptor {
                    Name = "P.WebPaymentTransientPage",
                    Priority = 5,
                    Route = new Route(
                       "p/web-payment",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Web.Payment"}, // this is the name of your module
                            {"controller", "ClientWebPayment"},
                            {"action", "WebPaymentTransientPage"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Web.Payment"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "GetWebPayDirectModel",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "p/request-pay-direct-web", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Web.Payment"}, // this is the name of your module
                            {"controller", "ClientWebPayment"},
                            {"action", "GetWebPayDirectModel"},
                            },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Web.Payment"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "PayDirectPaymentResponse",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                        "p/response-pay-direct-web", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Web.Payment"}, // this is the name of your module
                            {"controller", "ClientWebPayment"},
                            {"action", "PayDirectPaymentResponse"},
                            },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Web.Payment"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
            };
        }
    }
}