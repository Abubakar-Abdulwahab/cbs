using Orchard.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Payment.ClientWeb
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
                    Name = "PaymentClient.Default",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "ReturnToMerchantSite"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                  new RouteDescriptor {
                    Name = "PaymentClient.Notification",
                    Priority = 5,
                    Route = new Route(
                       "c/notify/{paymentRef}",
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "Notify"},

                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                     Name = "PaymentClient.C.MakePayment.Invoice",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/make-payment/{invoiceNumber}", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"}, // this is the name of your module
                            {"controller", "Collection"},
                            {"action", "MakePaymentByInvoice"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "PaymentClient.PaymentReference.Number",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/getreferencenumber", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "GetReferenceNumber"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"} // this is the name of your module
                        },
                        new MvcRouteHandler())//get-reference-number
                },
                new RouteDescriptor {
                     Name = "PaymentClient.Payment.Notification",
                    Priority = 5, //view rev head hierarchy
                    Route = new Route(
                       "c/x/sendnotification", // this is the name of the page url
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"}, // this is the name of your module
                            {"controller", "CollectionAjax"},
                            {"action", "SendPaymentNotification"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Parkway.CBS.Payment.ClientWeb"} // this is the name of your module
                        },
                        new MvcRouteHandler())
                },
            };
        }
    }
}